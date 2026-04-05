using UnityEngine;

public class ComboFireEffect : MonoBehaviour
{
    [Header("Particle System")]
    public ParticleSystem fireParticles;

    [Header("Niveles de fuego")]
    public int comboNivel1 = 10;  // Fuego pequeño
    public int comboNivel2 = 20;  // Fuego medio
    public int comboNivel3 = 30;  // Fuego máximo

    private int perfectStreak = 0;

    public static ComboFireEffect Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (fireParticles != null)
            fireParticles.Stop();
    }

    public void OnPerfectHit(int currentCombo)
    {
        perfectStreak = currentCombo; // Sincronizamos con el combo real
        UpdateFire();
    }

    public void OnMissOrNonPerfect()
    {
        perfectStreak = 0;
        StopFire();
    }

    private void UpdateFire()
    {
        if (fireParticles == null) return;

        var emission = fireParticles.emission;
        var main = fireParticles.main;

        if (perfectStreak >= comboNivel3)
        {
            // Fuego máximo - rojo/amarillo intenso
            emission.rateOverTime = 60;
            main.startColor = new Color(1f, 0.3f, 0f);
            main.startSize = 2.5f;
            if (!fireParticles.isPlaying) fireParticles.Play();
        }
        else if (perfectStreak >= comboNivel2)
        {
            // Fuego medio - naranja
            emission.rateOverTime = 40;
            main.startColor = new Color(1f, 0.5f, 0f);
            main.startSize = 1.8f;
            if (!fireParticles.isPlaying) fireParticles.Play();
        }
        else if (perfectStreak >= comboNivel1)
        {
            // Fuego pequeño - amarillo
            emission.rateOverTime = 10;
            main.startColor = new Color(1f, 0.8f, 0f);
            main.startSize = 1.2f;
            if (!fireParticles.isPlaying) fireParticles.Play();
        }
        else
        {
            StopFire();
        }
    }

    private void StopFire()
    {
        if (fireParticles != null)
            fireParticles.Stop();
        perfectStreak = 0;
    }

    public void ResetFire()
    {
        if (fireParticles != null)
        {
            fireParticles.Stop();
            fireParticles.Clear(); // Esto borra las partículas que ya están en el aire
        }
        perfectStreak = 0;
    }
}