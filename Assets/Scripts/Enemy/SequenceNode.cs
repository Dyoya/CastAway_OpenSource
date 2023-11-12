using System.Collections.Generic;

/*
    - �ڽ� ��� ����->���������� ����
    - �ڽ� ���� �߿� ������ ��尡 ���� ������ ����
    - ���� �ൿ�� ������� �����ؾ� �� �� ���
 */

public sealed class SequenceNode : INode
{
    List<INode> _childs;

    public SequenceNode(List<INode> childs)
    {
        _childs = childs;
    }

    // running ���¿����� ���� ���� �̵����� ����
    // ���� ��尡 success�� ��� ���� ��� ����
    public INode.ENodeState Evaluate()
    {
        // �ڽ��� ���ų� ��������� Failure ��ȯ
        if (_childs == null || _childs.Count == 0)
            return INode.ENodeState.ENS_Failure;

        // ��尡 Running�� ���� ������ �Ǵ��ϰ� ��� ��ȯ
        // ��尡 Success�� ���� ��� ����
        // ��尡 Failure�� ���з� �Ǵ��ϰ� ��� ��ȯ
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
