namespace OoMapper.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Reflection.Emit;

	public class DynamicMapperBuilder
	{
		public static DynamicMapperBuilder Create()
		{
			AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("DynamicMappers"), AssemblyBuilderAccess.RunAndSave);
			var moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicMappers", "DynamicMappers.dll");
			return new DynamicMapperBuilder(moduleBuilder);
		}

		private static readonly Type ParentType = typeof(DynamicMapperBase);
		private readonly ModuleBuilder moduleBuilder;
		private readonly MethodInfo compileMethod = ParentType.GetMethod("Compile", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);

		private DynamicMapperBuilder(ModuleBuilder moduleBuilder)
		{
			this.moduleBuilder = moduleBuilder;
		}

		public Type CreateDynamicMapper(IEnumerable<ITypePair> typeMaps)
		{
			TypeBuilder mapperBuilder = moduleBuilder.DefineType(string.Format("M{0}", Guid.NewGuid()), TypeAttributes.Public, ParentType);

			ConstructorBuilder constructorBuilder = mapperBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] {typeof (IMappingConfiguration)});
			ILGenerator constructorGenerator = constructorBuilder.GetILGenerator();
			constructorGenerator.Emit(OpCodes.Ldarg_0);
			constructorGenerator.Emit(OpCodes.Call, ParentType.GetConstructor(Type.EmptyTypes));

			foreach (var typeMap in typeMaps)
			{
				Type filedType = typeof (Func<,>).MakeGenericType(typeMap.SourceType, typeMap.DestinationType);
				FieldBuilder fieldBuilder = mapperBuilder.DefineField(string.Format("x{0}", Guid.NewGuid()), filedType, FieldAttributes.Private);
				constructorGenerator.Emit(OpCodes.Ldarg_0);
				constructorGenerator.Emit(OpCodes.Ldarg_1);
				constructorGenerator.Emit(OpCodes.Call, compileMethod.MakeGenericMethod(typeMap.SourceType, typeMap.DestinationType));
				constructorGenerator.Emit(OpCodes.Stfld, fieldBuilder);

				MethodBuilder methodBuilder = mapperBuilder.DefineMethod("Map", MethodAttributes.Public, typeMap.SourceType, new[] {typeMap.DestinationType});
				var methodGenerator = methodBuilder.GetILGenerator();
				methodGenerator.Emit(OpCodes.Ldarg_0);
				methodGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
				methodGenerator.Emit(OpCodes.Ldarg_1);
				methodGenerator.Emit(OpCodes.Callvirt, filedType.GetMethod("Invoke"));
				methodGenerator.Emit(OpCodes.Ret);
			}

			constructorGenerator.Emit(OpCodes.Ret);

			return mapperBuilder.CreateType();
		}
	}
}