namespace Server;

public class SessionManager
{
    private static SessionManager _session = new SessionManager();
    public static SessionManager Instance => _session;

    private int _sessionID = 0;
    private Dictionary<int, ClientSession> _sessionDic = new Dictionary<int, ClientSession>();
    private object _lock = new object();

    public ClientSession Generate()
    {
        lock (_lock)
        {
            int sessionID = ++_sessionID;

            ClientSession session = new ClientSession();
            session.SessionID = sessionID;
            _sessionDic.Add(sessionID, session);

            Console.WriteLine($"Connected : {sessionID}");

            return session;
        }
    }

    public ClientSession Find(int id)
    {
        lock (_lock)
        {
            ClientSession session = null;
            _sessionDic.TryGetValue(id, out session);

            return session;
        }
    }

    public void Remove(ClientSession session)
    {
        lock (_lock)
        {
            _sessionDic.Remove(session.SessionID);
        }
    }
}