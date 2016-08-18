namespace UserStorage.Interfaces.Generators
{
    /// <summary>
    /// Used to generate ids.
    /// </summary>
    public interface IIdGenerator
    {
        /// <summary>
        /// Returns current id from sequence.
        /// </summary>
        int CurrentId { get; }

        /// <summary>
        /// Generates the next id from sequence.
        /// </summary>
        /// <returns>
        /// Whether the next id exists.
        /// </returns>
        bool GenerateNextId();

        /// <summary>
        /// Sets a value which the generator has to start from.
        /// </summary>
        /// <param name="initialValue">
        /// Starting value.
        /// </param
        void SetInitialValue(int initialValue);
    }
}
