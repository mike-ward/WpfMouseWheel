namespace Logitech
{
    public delegate TResult RefFunc<R, T, TResult>(ref R reference, T parameter);
}