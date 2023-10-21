using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PinkSlip;

// プレイヤーからの入力をカーコントローラに適用するクラス
public class PlayerCarInput : MonoBehaviour
{
	// 対象コントローラ
	public CarController controller;

	[SerializeField]
	private Powertrain powertrain;

	private void Start()
	{
		powertrain.Gear = 1;
	}

	// 毎フレームの処理
	void Update()
	{
		//controller.Gear = 5;
		//powertrain.Gear = 1;
		//var g = powertrain.MaxTorqueGear();
		//print($"g {g}");
		//powertrain.Gear = g;

		//print($"RPM {powertrain.RPM}");

		controller.Gas = Mathf.Clamp01(Input.GetAxis("Vertical"));

		controller.Brake = Mathf.Clamp01(-Input.GetAxis("Vertical"));

		controller.SteerRate = Mathf.Clamp(Input.GetAxis("Horizontal"), -1f, 1f);

		//controller.InputBrake(-Input.GetAxis("Vertical"));


		//controller.InputSteering(Input.GetAxis("Horizontal"));

		if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			controller.Gear = powertrain.Gear + 1;
		}
		else if (Input.GetKeyDown(KeyCode.LeftControl))
		{
			controller.Gear = powertrain.Gear - 1;
		}

		powertrain.RemainingNitrous = 1f;
			powertrain.Nitrous = Input.GetKey(KeyCode.Keypad0);
		
	}
}