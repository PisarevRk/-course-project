using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FigureController : MonoBehaviour
{
    public FigureInfo figureInfo;
    TileController tile;

    [SerializeField]
    SpriteRenderer figureSprite;
    [SerializeField]
    Animator animator;
    [SerializeField]
    SpriteRenderer teamLabel;
    [SerializeField]
    TMPro.TextMeshProUGUI figureAmountText;
    [SerializeField]
    Transform onAttackEffectPivot;

    [SerializeField]
    GameObject onAttackEffect;

    bool rightOriented;

    public TileController Tile
    {
        get { return tile; }
        set { tile = value; }
    }



    [SerializeField]
    FigureInputController inputController;

    public SpriteRenderer SpriteRenderer
    {
        get { return figureSprite; }
    }

    public FigureInputController InputController
    {
        get { return inputController; }
    }

    public void Start()
    {
        if (figureInfo != null)
        {
            figureAmountText.text = figureInfo.UnitCount.ToString();
            rightOriented = figureInfo.Player;
            figureSprite.flipX = !rightOriented;

            if (figureInfo.UnitInfo.animatorController)
                animator.runtimeAnimatorController = figureInfo.UnitInfo.animatorController;

            if (!figureInfo.Player)
                teamLabel.color = new Color(1, 40f / 255, 0);
        }
    }

    /*public void SetFigureInfo(FigureInfo figureInfo)
    {
        this.figureInfo = figureInfo;


    }*/

    public void MakeMove(TileController tile)
    {
        FieldPathfinder.Path path = FieldPathfinder.FindPath(Tile.X, Tile.Y, tile.X, tile.Y);
        int pathLength = path.Length - 1;
        FigureController figure = tile.Figure;

        if (figureInfo.UnitInfo.ranged)
        {
            if (figure != null && IsEnemy(figure))
            {
                AttackAction(figure, true);
                return;
            }
        }
        if (pathLength <= figureInfo.movePointsRemaining)
        {
            if (figure is null)
                StartCoroutine(Move(path));
            else if (IsEnemy(figure))
            {
                if(pathLength != 1)
                {
                    StartCoroutine(MoveAndAttack(path));
                }
                else
                {
                    AttackAction(figure);
                }
            }
        }
    }

    public void AttackAction(FigureController enemy, bool ranged = false)
    {
        StartCoroutine(Attack(this, enemy, enemy.figureInfo.canAttack && !ranged));
        figureInfo.movePointsRemaining = 0;
    }

    public IEnumerator MoveAndAttack(FieldPathfinder.Path path)
    {
        figureInfo.movePointsRemaining -= path.Length - 1;
        SetTile(path.Nodes[1].Tile);

        for (int i = path.Length - 1; i > 0; i--)
        {
            yield return new WaitForSeconds(0.1f);
            TileController tile = path.Nodes[i].Tile;

            rightOriented = transform.position.x > tile.transform.position.x;
            figureSprite.flipX = rightOriented;
            transform.position = tile.transform.position;
        }

        AttackAction(path.Nodes[0].Tile.Figure);
    }

    public IEnumerator Move(FieldPathfinder.Path path)
    {
        figureInfo.movePointsRemaining -= path.Length - 1;
        SetTile(path.Nodes[0].Tile);

        for (int i = path.Length - 2; i >= 0; i--)
        {
            yield return new WaitForSeconds(0.1f);
            TileController tile = path.Nodes[i].Tile;

            rightOriented = transform.position.x > tile.transform.position.x;
            figureSprite.flipX = rightOriented;
            transform.position = tile.transform.position;
        }

        EventManager.InvokeMoveEnded();
    }

    private IEnumerator Attack(FigureController thisFigure, FigureController enemy, bool canAttack)
    {
        thisFigure.animator.SetTrigger("Attack");
        GameObject effect = Instantiate(onAttackEffect, enemy.onAttackEffectPivot.position, enemy.onAttackEffectPivot.rotation);

        yield return new WaitForSeconds(0.25f);

        Destroy(effect);

        enemy.GetDamagedBy(thisFigure);

        rightOriented = thisFigure.transform.position.x > enemy.transform.position.x;
        thisFigure.figureSprite.flipX = rightOriented;

        if (canAttack && enemy.figureInfo.UnitCount > 0)
        {
            enemy.figureInfo.canAttack = false;

            StartCoroutine(Attack(enemy, thisFigure, false));
        }
        else
            EventManager.InvokeMoveEnded();

    }

    public void GetDamagedBy(FigureController enemy)
    {
        int damage = Random.Range(enemy.figureInfo.UnitInfo.damageMin, enemy.figureInfo.UnitInfo.damageMax) * enemy.figureInfo.UnitCount;

        figureInfo.ReceiveDamage(damage);
        BattleController.gotDamaged.Add(this);

        figureAmountText.text = figureInfo.UnitCount.ToString();
    }

    public void MoveToTile(TileController tile)
    {
        rightOriented = transform.position.x > tile.transform.position.x;
        figureSprite.flipX = rightOriented;
        transform.position = tile.transform.position;

        SetTile(tile);
    }



    public void SetTile(TileController tile)
    {
        if (tile is null)
        {
            this.tile = null;
            return;
        }

        if(this.tile != null)
            this.tile.Figure = null;
        this.tile = tile;
        this.tile.Figure = this;
    }

    private bool IsEnemy(FigureController figure)
    {
        return this.figureInfo.Player != figure.figureInfo.Player;
    }
}