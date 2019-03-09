using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Speedcar;

public class CarUI : MonoBehaviour
{
	public Body Body;
	public Powertrain Powertrain;
	public Suspension Suspension;
	public WheelCollider left;
	public WheelCollider right;

	public Text speed;
	public Text gear;
	public Text L;
	public Text R;
	public Text Rpm;
	public Text flf;
	public Text frf;
	public Text fls;
	public Text frs;

	public Image rpm;
	public Image turbo;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		gear.text = Powertrain.Gear.ToString();
		speed.text = $"{Body.ForwardVelocity * 3.6f} km";

		L.text = left.steerAngle.ToString();
		R.text = right.steerAngle.ToString();

		Rpm.text = Powertrain.RPM.ToString();

		rpm.transform.rotation = Quaternion.Euler(0f, 0f, 180f - Mathf.Lerp(0f, 180f, Powertrain.RPM / 10000f));
		turbo.transform.rotation = Quaternion.Euler(0f, 0f, 180f - Mathf.Lerp(0f, 180f, Powertrain.Pressure / Powertrain.MaxPressure));
	}
}
