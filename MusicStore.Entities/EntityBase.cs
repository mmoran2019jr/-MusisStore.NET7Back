namespace MusicStore.Entities;

public class EntityBase
{
    //[Key] // Esto solo es necesario si la llave o PK no cumple con el nombre estandar
    public int Id { get; set; }

    public bool Status { get; set; }

    protected EntityBase()
    {
        Status = true;
    }
}