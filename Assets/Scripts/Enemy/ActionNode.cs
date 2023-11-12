using System;

/*
    행동을 정의한 노드 (리프 노드)
 */

public sealed class ActionNode : INode
{
    Func<INode.ENodeState> _onUpdate = null;

    public ActionNode(Func<INode.ENodeState> onUpdate)
    {
        _onUpdate = onUpdate;
    }

    // 값이 null이 아니면 해당 값 반환, 값이 null이면 failure 반환
    public INode.ENodeState Evaluate() => _onUpdate?.Invoke() ?? INode.ENodeState.ENS_Failure;
}