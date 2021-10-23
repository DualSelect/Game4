using KanKikuchi.AudioManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Attack : MonoBehaviour
{
    public Vector3 attacker;
    public Vector3 target;
    public Battle_Card attackCard;
    public Battle_Card blockCard;
    static Vector3 enemyLife = new Vector3(0, 650, 0);
    static Vector3 playerLife = new Vector3(0, -650, 0);
    static Vector3 playerUnit = new Vector3(-350, -100, 0);
    static Vector3 enemyUnit = new Vector3(350, 250, 0);
    public SimpleAnimation[] aAnimes;
    public SimpleAnimation[] dAnimes;
    public void AttackStart(Battle_Card card,int i)
    {
        attackCard = card;
        blockCard = null;
        SEManager.Instance.Play(SEPath.ATTACK);
        AttackEnd();
        if (card.own) attacker = playerUnit + new Vector3(200*i,0,0);
        else attacker = enemyUnit + new Vector3(-200 * i, 0, 0);
        gameObject.transform.localPosition = attacker;
        if (card.own) target = enemyLife;
        else target = playerLife;
        float angle = GetAngle(attacker, target);
        gameObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
        gameObject.SetActive(true);
        StartCoroutine(AttackAnime());
    }
    public void Block(Battle_Card card, int i)
    {
        blockCard = card;
        SEManager.Instance.Play(SEPath.BLOCK);
        AttackEnd();
        gameObject.transform.localPosition = attacker;
        if (card.own) target = playerUnit + new Vector3(200 * i, 0, 0);
        else target = enemyUnit + new Vector3(-200 * i, 0, 0);
        float angle = GetAngle(attacker, target);
        gameObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
        gameObject.SetActive(true);
        StartCoroutine(AttackAnime());
    }
    public IEnumerator Break(Battle_Card card, int i)
    {
        AttackEnd();
        if (card.own) attacker = playerUnit + new Vector3(200 * i, 0, 0);
        else attacker = enemyUnit + new Vector3(-200 * i, 0, 0);
        gameObject.transform.localPosition = attacker;
        if (card.own) target = enemyLife;
        else target = playerLife;

        float distance = (attacker - target).magnitude;
        SimpleAnimation aAnime = aAnimes[card.card.colorSort];
        aAnime.transform.localScale = new Vector3(distance / 2, 180, 1);
        aAnime.transform.localPosition = (attacker + target) / 2;
        float angle = GetAngle(attacker, target);
        if (card.card.colorSort == 0) angle += 180;
        aAnime.transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle - 90);

        aAnime.gameObject.SetActive(true);
        aAnime.Play();
        switch (card.card.colorSort)
        {
            case 0:
                SEManager.Instance.Play(SEPath.GATTACK);
                break;
            case 1:
                SEManager.Instance.Play(SEPath.RATTACK);
                break;
            case 2:
                SEManager.Instance.Play(SEPath.YATTACK);
                break;
            case 3:
                SEManager.Instance.Play(SEPath.PATTACK);
                break;
        }
        yield return new WaitForSecondsRealtime(1f);
        aAnime.gameObject.SetActive(false);
    }
    IEnumerator AttackAnime()
    {
        while (true) {
            gameObject.transform.localPosition = Vector3.MoveTowards(gameObject.transform.localPosition, target, 0.1f * Vector3.Distance(attacker,target));
            gameObject.transform.localScale = new Vector3(0.05f+ gameObject.transform.localScale.x, 1,1);
            yield return new WaitForSecondsRealtime(0.1f);
            if(gameObject.transform.localPosition == target)
            {
                gameObject.transform.localPosition = attacker;
                gameObject.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }
    public void AttackEnd()
    {
        gameObject.SetActive(false);
        StopAllCoroutines();
    }
    public IEnumerator Battle(Battle_Card attack, int a, Battle_Card defence, int d)
    {
        if (attack.own) attacker = playerUnit + new Vector3(200 * a, 0, -1);
        else attacker = enemyUnit + new Vector3(-200 * a - 50, 0, 0);
        if (defence.own) target = playerUnit + new Vector3(200 * d, 0, -1);
        else target = enemyUnit + new Vector3(-200 * d + 50, 0, 0);
        float distance = (attacker - target).magnitude;
        SimpleAnimation aAnime = aAnimes[attack.card.colorSort];
        SimpleAnimation dAnime = dAnimes[defence.card.colorSort];
        aAnime.transform.localScale = new Vector3(distance / 2, 180, 1);
        dAnime.transform.localScale = new Vector3(distance / 2, 180, 1);
        if (attack.card.colorSort == 0) aAnime.transform.localScale = new Vector3(distance / 2, 120, 1);
        if (defence.card.colorSort == 0) dAnime.transform.localScale = new Vector3(distance / 2, 120, 1);
        aAnime.transform.localPosition = new Vector3(-50, 0, 0) +(attacker + target) / 2;
        dAnime.transform.localPosition = new Vector3(50, 0, 0) + (attacker + target) / 2;


        float angle = GetAngle(attacker, target);
        if (attack.card.colorSort == 0) angle += 180;
        aAnime.transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle-90);
        angle = GetAngle(attacker, target);
        if (defence.card.colorSort == 0) angle += 180;
        dAnime.transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle+90);
        aAnime.gameObject.SetActive(true);
        dAnime.gameObject.SetActive(true);
        aAnime.Play();
        dAnime.Play();
        switch (attack.card.colorSort)
        {
            case 0:
                SEManager.Instance.Play(SEPath.GATTACK);
                break;
            case 1:
                SEManager.Instance.Play(SEPath.RATTACK);
                break;
            case 2:
                SEManager.Instance.Play(SEPath.YATTACK);
                break;
            case 3:
                SEManager.Instance.Play(SEPath.PATTACK);
                break;
        }
        switch (defence.card.colorSort)
        {
            case 0:
                SEManager.Instance.Play(SEPath.GATTACK);
                break;
            case 1:
                SEManager.Instance.Play(SEPath.RATTACK);
                break;
            case 2:
                SEManager.Instance.Play(SEPath.YATTACK);
                break;
            case 3:
                SEManager.Instance.Play(SEPath.PATTACK);
                break;
        }
        yield return new WaitForSecondsRealtime(1f);
        aAnime.gameObject.SetActive(false);
        dAnime.gameObject.SetActive(false);

    }
    public float GetAngle(Vector3 p1, Vector3 p2)
    {
        float dx = p2.x - p1.x;
        float dy = p2.y - p1.y;
        float rad = Mathf.Atan2(dy, dx);
        return rad * Mathf.Rad2Deg -90;
    }
    public void Move(int attack,int block)
    {
        if (attack >= 0)
        {
            if (attackCard.own) attacker = playerUnit + new Vector3(200 * attack, 0, 0);
            else attacker = enemyUnit + new Vector3(-200 * attack, 0, 0);
            gameObject.transform.localPosition = attacker;
        }
        if (block >= 0)
        {
            if (blockCard.own) target = playerUnit + new Vector3(200 * block, 0, 0);
            else target = enemyUnit + new Vector3(-200 * block, 0, 0);
        }
    }
}
