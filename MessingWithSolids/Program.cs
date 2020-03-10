using System;
using StereoKit;
using System.Collections.Generic;
using System.Linq;

namespace MessingWithSolids
{
	public delegate void ColliderFunc(Vec3 dimensions, float kilograms = 1, Vec3? offset = null);

	class ObjectSpawner
	{
		Model cubeModel;
		Model sphereModel;
		List<Model> spawnedModels = new List<Model>();
		string whichObject;
		Solid floor;
		List<Solid> objects = new List<Solid>();
		float objectScale = 0.25f;

		public ObjectSpawner()
		{
			whichObject = "cube";
			cubeModel = Model.FromMesh(
				Mesh.GenerateCube(Vec3.One),
				Default.Material
			);
			sphereModel = Model.FromMesh(
				Mesh.GenerateSphere(1f),
				Default.Material
			);
			floor = new Solid(new Vec3(0f, -1.5f, 0f), Quat.Identity, SolidType.Immovable);
			floor.AddBox(new Vec3(20f, 1f, 20f) * objectScale);
			spawnedModels.Add(Model.FromMesh(
				Mesh.GenerateCube(new Vec3(20f, 1f, 20f)),
				Default.Material
			));
			objects.Add(floor);
		}

		public void Update()
		{
			DisplayButtonPanel();
			SpawnObject();
		}

		private void SpawnObject()
		{
			Hand rightHand = Input.Hand(Handed.Right);
			Vec3 position;

			if (rightHand.IsJustPinched)
			{
				position = rightHand[FingerId.Index, JointId.Tip].position;
				position.y -= 0.2f;
				if (whichObject == "cube")
				{
					SpawnCube(position);
				}
				else if (whichObject == "sphere")
				{
					SpawnSphere(position);
				}
			}
			Pose solidPose;
			float c;
			for (int i = 0; i < objects.Count; i++)
			{
				c = i / (float)objects.Count;
				objects[i].GetPose(out solidPose);
				Renderer.Add(spawnedModels[i], solidPose.ToMatrix(objectScale), new Color(c, c, c));
			}
		}

		private void SpawnCube(Vec3 position)
		{
			objects.Add(new Solid(position, Quat.Identity));
			objects.Last().AddBox(Vec3.One * objectScale);
			spawnedModels.Add(cubeModel);
		}

		private void SpawnSphere(Vec3 position)
		{
			objects.Add(new Solid(position, Quat.Identity));
			objects.Last().AddSphere(1f * objectScale);
			spawnedModels.Add(sphereModel);
		}

		private void DisplayButtonPanel()
		{
			Vec3 headPosition = Input.Head.position;
			Vec3 windowPosition = new Vec3(0.4f, 0f, 0.1f);
			Pose windowPose = new Pose(windowPosition, Quat.LookAt(windowPosition, headPosition));

			UI.WindowBegin("Button Panel", ref windowPose, new Vec2(25f, 25f) * Units.cm2m, false);
			if (UI.Button("Cube"))
			{
				whichObject = "cube";
			}
			if (UI.Button("Sphere"))
			{
				whichObject = "sphere";
			}
			if (UI.Button("Reset"))
			{
				spawnedModels.Clear();
				objects.Clear();
				spawnedModels.Add(Model.FromMesh(
					Mesh.GenerateCube(new Vec3(20f, 1f, 20f)),
					Default.Material
				));
				objects.Add(floor);
			}
			if (UI.Button("Exit"))
			{
				StereoKitApp.Quit();
			}
			UI.WindowEnd();
		}
	}

	class Program
	{
		static void Main(string[] args)
		{
			if (!StereoKitApp.Initialize("MessingWithSolids", Runtime.MixedReality))
				Environment.Exit(1);

			ObjectSpawner ObjectSpawner = new ObjectSpawner();

			while (StereoKitApp.Step(() =>
			{
				ObjectSpawner.Update();
			}));

			StereoKitApp.Shutdown();
		}
	}
}
