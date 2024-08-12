namespace ServerCore;

public class ReceiveBuffer
{
    private ArraySegment<byte> _buffer;
    private int _readPosition;
    private int _writePosition;
    
    public int DataSize => _writePosition - _readPosition;
    public int FreeSize => _buffer.Count - _writePosition;
    
    public ReceiveBuffer(int bufferSize)
    {
        _buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);
    }
    
    // DataSegment
    public ArraySegment<byte> ReadSegment
        => new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _readPosition, DataSize);

    // ReceiveSegment
    public ArraySegment<byte> WriteSegment
        => new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _writePosition, FreeSize);

    public void Clear()
    {
        int dataSize = DataSize;
        if (dataSize == 0)
        {
            // 남은 데이터가 없으면 복사하지 않고 커서 위치만 리셋
            _readPosition = 0;
            _writePosition = 0;
        }
        else
        {
            // 남은것이 있으면 시작 위치로 복사
            Array.Copy(_buffer.Array, _buffer.Offset + _readPosition, _buffer.Array, _buffer.Offset, dataSize);
            _readPosition = 0;
            _writePosition = dataSize;
        }
    }

    public bool OnRead(int numOfBytes)
    {
        if (numOfBytes > DataSize)
            return false;

        _readPosition += numOfBytes;
        
        return true;
    }

    public bool OnWrite(int numOfBytes)
    {
        if (numOfBytes > FreeSize)
            return false;

        _writePosition += numOfBytes;
        
        return true;
    }
}