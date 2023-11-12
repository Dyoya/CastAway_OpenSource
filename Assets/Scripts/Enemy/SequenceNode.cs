using System.Collections.Generic;

/*
    - 자식 노드 왼쪽->오른쪽으로 실행
    - 자식 노드들 중에 실패한 노드가 있을 때까지 진행
    - 여러 행동을 순서대로 진행해야 할 때 사용
 */

public sealed class SequenceNode : INode
{
    List<INode> _childs;

    public SequenceNode(List<INode> childs)
    {
        _childs = childs;
    }

    // running 상태에서는 다음 노드로 이동하지 않음
    // 이전 노드가 success인 경우 다음 노드 실행
    public INode.ENodeState Evaluate()
    {
        // 자식이 없거나 비어있으면 Failure 반환
        if (_childs == null || _childs.Count == 0)
            return INode.ENodeState.ENS_Failure;

        // 노드가 Running면 실행 중으로 판단하고 즉시 반환
        // 노드가 Success면 다음 노드 진행
        // 노드가 Failure면 실패로 판단하고 즉시 반환
        foreach (var child in _childs)
        {
            switch (child.Evaluate())
            {
                case INode.ENodeState.ENS_Running:
                    return INode.ENodeState.ENS_Running;
                case INode.ENodeState.ENS_Success:
                    continue;
                case INode.ENodeState.ENS_Failure:
                    return INode.ENodeState.ENS_Failure;
            }
        }

        return INode.ENodeState.ENS_Success;
    }
}
