namespace dMobile.View.Types
{
    /// <summary>
    /// Defines the <see cref="Warehouse" />
    /// </summary>
    internal class Warehouse
    {
        /// <summary>
        /// Defines the id
        /// </summary>
        private int id;

        /// <summary>
        /// Defines the name
        /// </summary>
        private string name;

        /// <summary>
        /// Defines the displayName
        /// </summary>
        private string displayName;

        /// <summary>
        /// Initializes a new instance of the <see cref="Warehouse"/> class.
        /// </summary>
        public Warehouse()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Warehouse"/> class.
        /// </summary>
        /// <param name="id">The id<see cref="int"/></param>
        /// <param name="name">The name<see cref="string"/></param>
        public Warehouse(int id, string name)
        {
            this.Id = id;
            this.Name = name;
            this.DisplayName = this.Id + " " + this.Name;
        }

        /// <summary>
        /// Gets or sets the Id
        /// </summary>
        public int Id { get => this.id; set => this.id = value; }

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        public string Name { get => this.name; set => this.name = value; }

        /// <summary>
        /// Gets or sets the DisplayName
        /// </summary>
        public string DisplayName { get => this.displayName; set => this.displayName = value; }
    }
}
