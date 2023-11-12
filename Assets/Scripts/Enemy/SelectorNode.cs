using System.Collections.Generic;

/*
    - �ڽ� ��� ���� -> ���������� ����
    - ���� �ൿ �� �ϳ��� �����ؾ� �� �� ���
    - ������ ��尡 ������ �ش� ��� �����ϰ� ����
 */

public sealed class SelectorNode : INode
{
    List<INode> _childs;

    public SelectorNode(List<INode> childs)
    {
        _childs = childs;
    }

    // �ڽ� ��� �� ó������ Success�� Running ���¸� ���� ��尡 �߻��ϸ� ����
    public INode.ENodeState Evaluate()
    {
        // �ڽ��� ������ Failure ��ȯ
        if (_childs == null)
            return INode.ENodeState.ENS_Failure;

        // ó������ Success�� Running ���¸� ���� ��尡 �߻��ϸ� ��ȯ
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

        // Success�� Running ���¸� ���� ��尡 ������ Failure ��ȯ
        return INode.ENodeState.ENS_Failure;
    }
}