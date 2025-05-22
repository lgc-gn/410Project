using UnityEngine;

public class emission : MonoBehaviour
{   
    public ParticleSystem particle;
    public float startParticleEffect = 2.0f;
    float timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = 0.0f;
        particle.Stop();
        
        
    }
/*    void startParticleEffect()
    {
        set particle.emission True;
        set particle2.emission True;
    }
  */
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if((timer > startParticleEffect)&&(particle.isPlaying == false)){
            particle.Play();
        }
    }
}
