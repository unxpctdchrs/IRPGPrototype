using UnityEngine;

public class MeleeWeaponHitbox : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform _bladeBase;
    [SerializeField] private Transform _bladeTip;
    
    [SerializeField] private float _hitboxThickness = 0.3f;
    [SerializeField] private LayerMask _enemyLayer;
    private bool _hasHitThisAttack = false;

    public void ResetHitbox()
    {
        _hasHitThisAttack = false;
    }

    public Collider PerformHitCheck() 
    {
        if (_hasHitThisAttack) return null;

        Collider[] hitEnemies = Physics.OverlapCapsule(
            _bladeBase.position, 
            _bladeTip.position, 
            _hitboxThickness, 
            _enemyLayer
        );

        if (hitEnemies.Length > 0)
        {
            _hasHitThisAttack = true;
            return hitEnemies[0]; 
        }

        return null;
    }

    private void OnDrawGizmosSelected()
    {
        if (_bladeBase != null && _bladeTip != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_bladeBase.position, _hitboxThickness);
            Gizmos.DrawWireSphere(_bladeTip.position, _hitboxThickness);
            Gizmos.DrawLine(_bladeBase.position, _bladeTip.position);
        }
    }
}