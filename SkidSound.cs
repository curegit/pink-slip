using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SkidSound : MonoBehaviour
{
	public AudioSource AudioSource;

	public AudioClip AudioClip;

	public Speedcar.Suspension Suspension;

    // Start is called before the first frame update
    void Start()
    {
		AudioSource.clip = AudioClip;
		AudioSource.loop = true;
		AudioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
		float slip = Suspension.WheelHits.Average(h => h.HasValue ? h.Value.sidewaysSlip : 0f);
		AudioSource.volume = Mathf.Clamp01(Mathf.Abs(slip) * 2f);
    }
}
