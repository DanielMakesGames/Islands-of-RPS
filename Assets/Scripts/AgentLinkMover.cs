using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class AgentLinkMover : MonoBehaviour
{
    public enum OffMeshLinkMoveMethod
    {
        Teleport,
        NormalSpeed,
        Parabola,
        Curve
    }

    public OffMeshLinkMoveMethod myMoveMethod = OffMeshLinkMoveMethod.Parabola;
    public AnimationCurve myAnimationCurve = new AnimationCurve();

    const float parabolaHeight = 6f;
    const float parabolaTime = 0.3f;

    IEnumerator Start()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.autoTraverseOffMeshLink = false;
        while (true)
        {
            if (agent.isOnOffMeshLink)
            {
                if (Mathf.Abs(agent.currentOffMeshLinkData.endPos.y - transform.position.y) > 1f)
                {
                    yield return StartCoroutine(Parabola(agent, parabolaHeight, parabolaTime));
                }
                else
                {
                    switch (myMoveMethod)
                    {
                        case OffMeshLinkMoveMethod.NormalSpeed:
                            yield return StartCoroutine(NormalSpeed(agent));
                            break;
                        case OffMeshLinkMoveMethod.Parabola:
                            yield return StartCoroutine(Parabola(agent, parabolaHeight, parabolaTime));
                            break;
                        case OffMeshLinkMoveMethod.Curve:
                            yield return StartCoroutine(Curve(agent, 0.5f));
                            break;
                    }
                }
                if (agent.isOnOffMeshLink && agent.isActiveAndEnabled)
                {
                    agent.CompleteOffMeshLink();
                }
            }
            yield return null;
        }
    }

    IEnumerator NormalSpeed(NavMeshAgent agent)
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
        while (agent.transform.position != endPos)
        {
            agent.transform.position = Vector3.MoveTowards(agent.transform.position, endPos, agent.speed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator Parabola(NavMeshAgent agent, float height, float duration)
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
        float normalizedTime = 0.0f;
        while (normalizedTime < 1.0f)
        {
            float yOffset = height * 4.0f * (normalizedTime - normalizedTime * normalizedTime);
            agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }

        agent.transform.position = endPos;
    }

    IEnumerator Curve(NavMeshAgent agent, float duration)
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
        float normalizedTime = 0.0f;
        while (normalizedTime < 1.0f)
        {
            float yOffset = myAnimationCurve.Evaluate(normalizedTime);
            agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
    }
}
