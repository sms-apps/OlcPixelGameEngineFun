using System;
using System.Collections.Generic;
using PixelEngine;
using PixelEngine.Utilities;

namespace PopUpMenu
{
    public class MenuObject
    {
        protected const int Patch = 8;

        #region Properties

        /// <summary>
        /// The size of the padding between this MenuObject's cells.
        /// </summary>
        protected Vector2Int CellPadding { get; set; } = new Vector2Int(2, 0);

        /// <summary>
        /// The size of cells in this MenuObject.
        /// </summary>
        protected Vector2Int CellSize { get; set; }

        /// <summary>
        /// This MenuObject's "table".
        /// </summary>
        protected Vector2Int CellTable { get; set; }

        /// <summary>
        /// Is this MenuObject enabled?
        /// </summary>
        protected bool Enabled { get; set; } = true;

        /// <summary>
        /// This MenuObject's Id.
        /// </summary>
        protected int Id { get; set; } = -1;

        /// <summary>
        /// Dictionary to track the Items by index.
        /// </summary>
        protected Dictionary<string, uint> ItemPointer { get; set; } = new Dictionary<string, uint>();

        /// <summary>
        /// Children of this MenuObject.
        /// </summary>
        protected List<MenuObject> Items { get; set; } = new List<MenuObject>();

        /// <summary>
        /// This MenuObject's name.
        /// </summary>
        protected string Name { get; set; }

        /// <summary>
        /// Size of a single Patch.
        /// </summary>
        protected Vector2Int PatchSize { get; set; } = new Vector2Int(Patch, Patch);

        /// <summary>
        /// The size of this MenuObject, in Patches.
        /// </summary>
        protected Vector2Int SizeInPatches { get; set; } = new Vector2Int(0, 0);

        /// <summary>
        /// The top currently visible row for this MenuObject.
        /// </summary>
        protected int TopVisibleRow { get; set; }

        /// <summary>
        /// Total number of rows in this MenuObject.
        /// </summary>
        protected int TotalRows { get; set; }

        /// <summary>
        /// Overload of the [].
        /// </summary>
        /// <param name="name">Key name of the MenuObject to retrieve.</param>
        /// <returns>MenuObject value index by key.</returns>
        public MenuObject this[string key]
        {
            get
            {
                if (!ItemPointer.ContainsKey(key))
                {
                    ItemPointer[key] = (uint)Items.Count;
                    Items.Add(new MenuObject(key));
                }

                return Items[(int)ItemPointer[key]];
            }
        }

        #endregion Properties

        #region Construction

        /// <summary>
        /// ctor for MenuObject with default Name.
        /// </summary>
        public MenuObject() => Name = "root";

        /// <summary>
        /// ctor for MenuObject specifying Name.
        /// </summary>
        public MenuObject(string name) => Name = name;

        #endregion Construction

        #region Getters

        /// <summary>
        /// Get this MenuObject's size, in Vector2Int format.
        /// </summary>
        /// <remarks>For now, cells are simply one line strings.</remarks>
        /// <returns>Vector2Int</returns>
        public Vector2Int GetSize()
        {
            return new Vector2Int(Name.Length, 1);
        }

        /// <summary>
        /// Does this MenuObject contain any children in the Items collection?
        /// </summary>
        /// <returns>bool</returns>
        public bool HasChildren() => Items.Count > 0;

        #endregion Getters

        #region Setters

        /// <summary>
        /// Sets this MenuObject's Enabled property.
        /// </summary>
        /// <param name="enabled"></param>
        /// <remarks>
        /// Returns this MenuObject for chaining.
        /// </remarks>
        /// <returns>MenuObject (this).</returns>
        public MenuObject Enable(bool enabled)
        {
            Enabled = enabled;
            return this;
        }

        /// <summary>
        /// Set this MenuObject's Id.
        /// </summary>
        /// <remarks>
        /// Returns this MenuObject for chaining.
        /// </remarks>
        /// <param name="id">Value with which to set Id.</param>
        /// <returns>MenuObject (this).</returns>
        public MenuObject SetId(int id)
        {
            Id = id;
            return this;
        }

        /// <summary>
        /// Set this MenuObject's CellTable value.
        /// </summary>
        /// <remarks>
        /// Returns this MenuObject for chaining.
        /// </remarks>
        /// <param name="columns">Number of columns.</param>
        /// <param name="rows">Number of rows.</param>
        /// <returns>MenuObject (this).</returns>
        public MenuObject SetTable(int columns, int rows)
        {
            CellTable = new Vector2Int(columns, rows);
            return this;
        }

        #endregion Setters

        #region Actions

        public void Build()
        {
            // Recursively build all children so they can determine their size and use that size to indicate cell
            // sizes if this object contains more than one item.
            foreach (var item in Items)
            {
                if (item.HasChildren()) item.Build();

                // Longest child name determines cell width.
                CellSize = new Vector2Int(
                    Math.Max(item.GetSize().x, CellSize.x),
                    Math.Max(item.GetSize().y, CellSize.y)
                );
            }

            // Adjust the size of this object, in patches, if it were rendered as a panel.
            SizeInPatches = new Vector2Int(
                CellTable.x * CellSize.x + (CellTable.x - 1) * CellPadding.x + 2,
                CellTable.y * CellSize.y + (CellTable.y - 1) * CellPadding.y + 2
            );

            // Calculate how many rows this item has to hold.
            TotalRows = Items.Count / CellTable.x + (Items.Count % CellTable.x > 0 ? 1 : 0);
        }

        public void DrawSelf(Game game, Sprite sprite, Vector2Int screenOffset)
        {
            // Record current pixel mode user is using.
            Pixel.Mode currentPixelMode = game.PixelMode;
            game.PixelMode = Pixel.Mode.Mask;

            // Draw the Panel and Border.
            var patchPosition = new Vector2Int(0, 0);
            for (patchPosition.x = 0; patchPosition.x < SizeInPatches.x; patchPosition.x++)
            {
                for (patchPosition.y = 0; patchPosition.y < SizeInPatches.y; patchPosition.y++)
                {
                    // Determine position in screen space.
                    var screenLocation = patchPosition * Patch + screenOffset;

                    // Calculate which patch is needed.
                    var sourcePatch = new Vector2Int(0, 0);
                    if (patchPosition.x > 0) sourcePatch.x = 1;
                    if (patchPosition.x == SizeInPatches.x - 1) sourcePatch.x = 2;
                    if (patchPosition.y > 0) sourcePatch.y = 1;
                    if (patchPosition.y == SizeInPatches.y - 1) sourcePatch.y = 2;

                    // Draw the actual Patch.
                    game.DrawPartialSprite(screenLocation, sprite, sourcePatch * Patch, PatchSize.x, PatchSize.y);
                }
            }

            // === Draw Panel Contents
            var cell = new Vector2Int(0, 0);
            patchPosition = new Vector2Int(1, 1);

            // Work out visible items
            var topLeftItem = TopVisibleRow * CellTable.x;
            var bottomRightItem = CellTable.y * CellTable.x + topLeftItem;

            // Clamp to the size of the child item vector.
            bottomRightItem = Math.Min(Items.Count, bottomRightItem);
            var visibleItems = bottomRightItem - topLeftItem;

            // Draw visible items.
            for (var i = 0; i < visibleItems; i++)
            {
                // Cell location.
                cell = new Vector2Int(
                    i % CellTable.x,
                    i / CellTable.x
                );

                // Patch location, including border offset and padding.
                patchPosition = new Vector2Int(
                    cell.x * (CellSize.x + CellPadding.x) + 1,
                    cell.y * (CellSize.y + CellPadding.y) + 1
                );

                // Actual screen location in pixels.
                var screenLocation = patchPosition * Patch + screenOffset;

                // Display item header.
                game.DrawText(
                    screenLocation,
                    Items[topLeftItem + i].Name,
                    Items[topLeftItem + i].Enabled ? Pixel.Presets.White : Pixel.Presets.DarkGrey
                );
            }
            game.PixelMode = currentPixelMode;
        }

        #endregion Actions
    }
}