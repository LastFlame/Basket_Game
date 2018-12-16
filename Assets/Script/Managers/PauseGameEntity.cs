using System.Linq;

public interface IPauseEntity
{
    bool IsPaused { get; }
    void OnPause();
    void OnUnPause();
}

public class PauseGameEntity : SingletonEntity<PauseGameEntity>
{
    public void PauseAllEntity()
    {
        
        ChangeIPauseEntityStatus(FindIPauseEntities(), true);
    }

    public void UnPauseAllEntity()
    {
        ChangeIPauseEntityStatus(FindIPauseEntities(), false);
    }

    public void PauseEntity(IPauseEntity entity)
    {
        if(entity.IsPaused)
        {
            return;
        }

        entity.OnPause();
    }

    public void UnPauseEntity(IPauseEntity entity)
    {
        if(!entity.IsPaused)
        {
            return;
        }

        entity.OnUnPause();
    }

    private IPauseEntity[] FindIPauseEntities()
    {
       return FindObjectsOfType<BaseEntity>()
              .OfType<IPauseEntity>()
              .ToArray();
    }

    private void ChangeIPauseEntityStatus(IPauseEntity[] pauseEntities, bool pause)
    {
        for(int i = 0; i < pauseEntities.Length; ++i)
        {
            if (pause)
            {
                pauseEntities[i].OnPause();
                continue;
            }

            pauseEntities[i].OnUnPause();
        }
    }
}
