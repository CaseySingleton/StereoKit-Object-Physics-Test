using System;
using StereoKit;

namespace ObjectsWithPhysics
{
	class Program
	{
		class Cube
		{
			float _size; // The length of each size of the cube
			float _scale; // The scale of the size
			Color _color; // Color of the cube
			Model _cubeModel; // This is what gets rendered by the Renderer
			Solid _cubeSolid; // This is what holds position and rotation information of the cubes Model
			Vec3 _spawnPosition; // Position in which to spawn the cube

			/// <summary>
			/// Creates a basic cube one meter in size and spawning at position world 0
			/// </summary>
			public Cube()
			{
				_size = 1f;
				_scale = 1f;
				_color = Color.White;
				_spawnPosition = Vec3.Zero;
				GenerateCube();
			}

			/// <summary>
			/// Creates a cube object
			/// </summary>
			/// <param name="size">Size of the cube in meters</param>
			/// <param name="scale">Scale of the cube</param>
			/// <param name="color">Color of the cube</param>
			/// <param name="spawnPosition">Location to spawn cube</param>
			public Cube(float size, float scale, Color color, Vec3 spawnPosition)
			{
				_size = size;
				_scale = scale;
				_color = color;
				_spawnPosition = spawnPosition;
				GenerateCube();
			}

			/// <summary>
			/// Creates the Model and Solid attributes of the cube
			/// </summary>
			private void GenerateCube()
			{
				_cubeModel = Model.FromMesh(
					Mesh.GenerateCube(Vec3.One * _size),
					Default.Material
				);
				_cubeSolid = new Solid(_spawnPosition, Quat.Identity);
			}

			/// <summary>
			/// Adds the cubes Model to the Renderes stack
			/// </summary>
			public void Update()
			{
				Pose solidPose; // I think this holds a Vec3 position and a Quat rotation

				_cubeSolid.GetPose(out solidPose); // The out keyword is like passing the address of solidPose
				Renderer.Add(_cubeModel, solidPose.ToMatrix(_scale)); // Adds the cubes Model to the render stack
			}

			/// <summary>
			/// Sets the position of the cube
			/// </summary>
			/// <param name="position">The new position of the cube</param>
			public void SetPosition(Vec3 position)
			{
				_cubeSolid.Move(position, Quat.Identity);
			}
			
			/// <summary>
			/// Sets the scale of the cube
			/// </summary>
			/// <param name="scale">The new scale of the cube</param>
			public void SetScale(float scale)
			{
				_scale = scale;
			}
		}
		static void Main(string[] args)
		{
			// Initialize StereoKit
			if (!StereoKitApp.Initialize("ObjectsWithPhysics", Runtime.MixedReality))
				Environment.Exit(1);

			// Create a cube object
			Cube cube = new Cube(1f, 0.25f, Color.White, new Vec3(0f, 1f, -1f));

			// Game/Render/Logic loop or something
			while (StereoKitApp.Step(() =>
			{
				Hand rightHand = Input.Hand(Handed.Right); // Getting the right hand
				if (rightHand.IsPinched) // If the user is pinching their right hand...
				{
					// ... Then set the cubes current position to the location of the users right index finger tip
					cube.SetPosition(rightHand[FingerId.Index, JointId.Tip].position);
				}
				cube.Update();
			})) ;

			StereoKitApp.Shutdown();
		}
	}
}
