using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class DamageZone : NetworkBehaviour
{
    [SerializeField] private Sprite damageSprite = null;
    private SpriteRenderer spriteRenderer;

    [Networked] private TickTimer _despawnTimer { get; set; }
    [Networked] private TickTimer _damageTimer { get; set; }
    [Networked] private NetworkBool expired { get; set; }
    [Networked] private NetworkBool givesDamage { get; set; }

    public override void Spawned()
    {
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        expired = false;
        givesDamage = false;

        if (!Object.HasStateAuthority) return;

        _despawnTimer = TickTimer.CreateFromSeconds(Runner, 4f);
        _damageTimer = TickTimer.CreateFromSeconds(Runner, 2f);
    }
    
    public override void FixedUpdateNetwork()
    {
       CheckZoneExpiration();
       CheckZoneDamage();
    }

    private void CheckZoneExpiration() {
        if(Object == null) return;
        if(!Object.HasStateAuthority) return;
        if(expired) return;

        if(!_despawnTimer.Expired(Runner)) return;
        expired = true;
    }

    private void CheckZoneDamage() {
        if(Object == null) return;
        if(!Object.HasStateAuthority) return;
        if(givesDamage) return;

        if(!_damageTimer.Expired(Runner)) return;
        givesDamage = true;
    }

    public override void Render()
    {
        if(givesDamage) {
            spriteRenderer.sprite = damageSprite;
        }
        if (expired)
        {
           Runner.Despawn(Object);
        }
    }

    public bool GivesDamage() {
        if(Object == null) return false;

        return givesDamage;
    }
}
