using System;

/*
    �ൿ�� ������ ��� (���� ���)
 */

public sealed class ActionNode : INode
{
    Func<INode.ENodeState> _onUpdate = null;

    public ActionNode(Func<INode.ENodeState> onUpdate)
    {
        _onUpdate = onUpdate;
    }

    // ���� null�� �ƴϸ� �ش� �� ��ȯ, ���� null�̸� failure ��ȯ
    public INode.ENodeState Evaluate() => _onUpdate?.Invoke() ?? INode.ENodeState.ENS_Failure;
}