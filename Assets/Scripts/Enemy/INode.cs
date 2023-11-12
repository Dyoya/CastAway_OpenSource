/*
    노드 인터페이스
*/
public interface INode
{
    public enum ENodeState
    {
        ENS_Running,
        ENS_Success,
        ENS_Failure,
    }

    // 노드가 어떤 상태인지 반환
    public ENodeState Evaluate();
}