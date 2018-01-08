public interface IHordeAbleWork : IWork {

    /// <summary>
    /// Compares self to another work object in relation to a horde network. Used to determine whether a zombie within a horde should change behaviour due to difference
    /// </summary>
    /// <param name="other">Other work object to compare self to</param>
    /// <returns>Whether the two work objects are identical</returns>
    bool Compare(IWork other);
}
