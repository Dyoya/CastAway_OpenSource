/*
    ��� �������̽�
*/
public interface INode
{
    public enum ENodeState
    {
        ENS_Running,
        ENS_Success,
        ENS_Failure,
    }

    // ��尡 � �������� ��ȯ
    public ENodeState Evaluate();
}