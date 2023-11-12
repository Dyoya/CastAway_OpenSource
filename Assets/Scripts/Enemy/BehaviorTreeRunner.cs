/*
    �ൿ Ʈ��(Behavior Tree) ������ �� ���
 */
public class BehaviorTreeRunner
{
    // ��Ʈ ���
    INode _rootNode;

    public BehaviorTreeRunner(INode rootNode)
    {
        _rootNode = rootNode;
    }

    // �ൿ Ʈ�� ����
    public void Operate()
    {
        // ��Ʈ ��忡�� ���� �� ����� Evaluate �Լ� ȣ�� -> ��ü Ʈ�� ����
        _rootNode.Evaluate();
    }
}