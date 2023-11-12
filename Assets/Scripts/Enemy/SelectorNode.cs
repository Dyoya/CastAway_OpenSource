using System.Collections.Generic;

/*
    - 자식 노드 왼쪽 -> 오른쪽으로 진행
    - 여러 행동 중 하나만 실행해야 할 때 사용
    - 성공한 노드가 있으면 해당 노드 실행하고 종료
 */

public sealed class SelectorNode : INode
{
    List<INode> _childs;

    public SelectorNode(List<INode> childs)
    {
        _childs = childs;
    }

    // 자식 노드 중 처음으로 Success나 Running 상태를 가진 노드가 발생하면 멈춤
    public INode.ENodeState Evaluate()
    {
        // 자식이 없으면 Failure 반환
        if (_childs == null)
            return INode.ENodeState.ENS_Failure;

        // 처음으로 Success나 Running 상태를 가진 노드가 발생하면 반환
        foreach (var child in _childs)
        {
            switch(child.Evaluate())
            {
                case INode.ENodeState.ENS_Running:
                    return INode.ENodeState.ENS_Running;
                case INode.ENodeState.ENS_Success:
                    return INode.ENodeState.ENS_Success;
            }
        }

        // Success나 Running 상태를 가진 노드가 없으면 Failure 반환
        return INode.ENodeState.ENS_Failure;
    }
}