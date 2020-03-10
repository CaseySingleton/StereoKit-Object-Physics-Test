using System;
using StereoKit;

public class Grabbable
{
	Model _model;
	Bounds _bounds;
	Color _color;
	Vec3 _position;
	Vec3 _offset;
	bool _grabbing;

	/// <summary>
	/// Creates a grabbable object refrencing a given model
	/// </summary>
	/// <param name="model">The model to make grabbable</param>
	public Grabbable(ref Model model)
	{
		_model = model;
		_bounds = model.Bounds;
		_color = Color.White;
		_position = Vec3.Zero;
		_offset = Vec3.Zero;
		_grabbing = false;
	}

	/// <summary>
	/// Creates a grabbable object refrencing a given model
	/// </summary>
	/// <param name="model">The model to make grabbable</param>
	/// <param name="position">The starting position of the model</param>
	public Grabbable(ref Model model, Vec3 position)
	{
		_model = model;
		_bounds = model.Bounds;
		_color = Color.White;
		_position = position;
		_offset = Vec3.Zero;
		_grabbing = false;
	}

	/// <summary>
	/// Creates a grabbable object refrencing a given model
	/// </summary>
	/// <param name="model">The model to make grabbable</param>
	/// <param name="color">The color of the model</param>
	public Grabbable(ref Model model, Color color)
	{
		_model = model;
		_bounds = model.Bounds;
		_color = color;
		_position = Vec3.Zero;
		_offset = Vec3.Zero;
		_grabbing = false;
	}

	/// <summary>
	/// Creates a grabbable object refrencing a given model
	/// </summary>
	/// <param name="model">The model to make grabbable</param>
	/// <param name="position">The starting position of the model</param>
	/// <param name="color">The color of the model</param>
	public Grabbable(ref Model model, Vec3 position, Color color)
	{
		_model = model;
		_bounds = model.Bounds;
		_color = color;
		_position = position;
		_offset = Vec3.Zero;
		_grabbing = false;
	}

	public void Update()
	{
		Hand hand;
		Pose fingertip;

		for (int i = 0; i < (int)Handed.Max; i++) // For each hand
		{
			hand = Input.Hand((Handed)i); // Get the current hand
			if (hand.IsTracked)
			{
				for (int j = 1; j < 5; j++) // For each finger (excluding thumb)
				{
					fingertip = hand[(FingerId)j, JointId.Tip].Pose; // Get the fingertips pose information
					if (_bounds.Contains(fingertip.position)) // If the fingertip is in the models boundary
					{
						if (hand.IsJustPinched || hand.IsJustGripped) // If the user jsut pinched or just grabbed...
						{
							_grabbing = true; // ...the user is currently grabbing
							_offset = _position - fingertip.position;
						}
						if (hand.IsJustUnpinched || hand.IsJustUngripped) // If the user stopped pinching or grabbing...
						{
							_grabbing = false; // ...the user is no longer grabbing
						}
						if (_grabbing == true) // If we are currently grabbing
						{
							_position = fingertip.position + _offset; // Update the objects position relative to the grabbed position
						}
						break ;
					}
					else
					{
						_grabbing = false;
					}
				}
			}
		}
		_bounds.center = _position; // Update the position of the models boundary
		_model.Draw(Matrix.T(_position), _color); // Draw the model at its new position
	}
}
