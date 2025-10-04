namespace GoF.Iterator;

public class Range : IEnumerable<int>
{
    public IEnumerator<int> GetEnumerator(){ for(int i=0;i<3;i++) yield return i; }
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()=>GetEnumerator();
}
