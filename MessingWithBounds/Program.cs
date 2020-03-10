using System;
using StereoKit;

namespace MessingWithBounds
{
	class Program
	{
		static void Main(string[] args)
		{
			Model cubeModel;
			Model sphereModel;
			if (!StereoKitApp.Initialize("MessingWithBounds", Runtime.MixedReality))
				Environment.Exit(1);

			// Creating models
			cubeModel = Model.FromMesh(
				Mesh.GenerateCube(Vec3.One * 0.25f),
				Default.Material
			);
			sphereModel = Model.FromMesh(
				Mesh.GenerateSphere(0.15f),
				Default.Material
			);

			// Creating grabbable versions of the above models
			Grabbable grabbableCube = new Grabbable(ref cubeModel, new Vec3(-0.25f, 0f, 0f));
			Grabbable grabbableSphere = new Grabbable(ref sphereModel, new Vec3(0.25f, 0f, 0f));

			while (StereoKitApp.Step(() =>
			{
				grabbableCube.Update();
				grabbableSphere.Update();
			})) ;

			StereoKitApp.Shutdown();
		}
	}
}
