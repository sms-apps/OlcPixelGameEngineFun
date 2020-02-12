using System;
using System.Collections.Generic;
using PixelEngine;
using PixelEngine.Utilities;

namespace PopUpMenuSystem
{
    public class MenuObject
    {
        public static int Patch = 8;

        #region Properties

        protected Vector2Int CellCursor { get; set; } = new Vector2Int(0, 0);

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

        protected int CursorItem { get; set; } = 0;

        public Vector2Int CursorPos { get; set; } = new Vector2Int(0, 0);

        /// <summary>
        /// Is this MenuObject enabled?
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// This MenuObject's Id.
        /// </summary>
        public int Id { get; set; } = -1;

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
        public string Name { get; set; }

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
        /// Gets the currently selected item.
        /// </summary>
        /// <returns>MenuObject that is currently at the index of CursorItem.</returns>
        public MenuObject GetSelectedItem() => Items[CursorItem];

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
                CellSize = new Vector2Int(Math.Max(item.GetSize().x, CellSize.x), Math.Max(item.GetSize().y, CellSize.y));
            }

            // Adjust the size of this object, in patches, if it were rendered as a panel.
            SizeInPatches = new Vector2Int(
                CellTable.x * CellSize.x + (CellTable.x - 1) * CellPadding.x + 2,
                CellTable.y * CellSize.y + (CellTable.y - 1) * CellPadding.y + 2
            );

            // Calculate how many rows this item has to hold.
            TotalRows = Items.Count / CellTable.x + (Items.Count % CellTable.x > 0 ? 1 : 0);
        }

        public void ClampCursor()
        {
            // Find item in children.
            CursorItem = CellCursor.y * CellTable.x + CellCursor.x;

            // Clamp cursor.
            if (CursorItem >= Items.Count)
            {
                CellCursor = new Vector2Int((Items.Count % CellTable.x) - 1, Items.Count / CellTable.x);
                CursorItem = Items.Count - 1;
            }
        }

        public void DrawSelf(Game game, Sprite sprite, Vector2Int screenOffset)
        {
            var screenLocation = new Vector2Int(0, 0);
            var sourcePatch = new Vector2Int(0, 0);
            var cell = new Vector2Int(0, 0);
            var patchPosition = new Vector2Int(0, 0);

            // Record current pixel mode user is using.
            Pixel.Mode currentPixelMode = game.PixelMode;
            game.PixelMode = Pixel.Mode.Mask;

            // Draw the Panel and Border.
            for (patchPosition.x = 0; patchPosition.x < SizeInPatches.x; patchPosition.x++)
            {
                for (patchPosition.y = 0; patchPosition.y < SizeInPatches.y; patchPosition.y++)
                {
                    // Determine position in screen space.
                    screenLocation = patchPosition * Patch + screenOffset;

                    // Calculate which patch is needed.
                    sourcePatch = new Vector2Int(0, 0);
                    if (patchPosition.x > 0) sourcePatch.x = 1;
                    if (patchPosition.x == SizeInPatches.x - 1) sourcePatch.x = 2;
                    if (patchPosition.y > 0) sourcePatch.y = 1;
                    if (patchPosition.y == SizeInPatches.y - 1) sourcePatch.y = 2;

                    // Draw the actual Patch.
                    game.DrawPartialSprite(screenLocation, sprite, sourcePatch * Patch, PatchSize.x, PatchSize.y);
                }
            }

            #region === Draw Panel Contents ===

            // Work out visible items
            var topLeftItem = TopVisibleRow * CellTable.x;
            var bottomRightItem = CellTable.y * CellTable.x + topLeftItem;

            // Clamp to the size of the child item vector.
            bottomRightItem = Math.Min(Items.Count, bottomRightItem);
            var visibleItems = bottomRightItem - topLeftItem;

            // Draw the top scroll marker, if required.
            if (TopVisibleRow > 0)
            {
                patchPosition = new Vector2Int(SizeInPatches.x - 2, 0);
                screenLocation = patchPosition * Patch + screenOffset;
                sourcePatch = new Vector2Int(3, 0);
                game.DrawPartialSprite(screenLocation, sprite, sourcePatch * Patch, PatchSize.x, PatchSize.y);
            }

            // Draw the bottom scroll marker, if required.
            if ((TotalRows - TopVisibleRow) > CellTable.y)
            {
                patchPosition = new Vector2Int(SizeInPatches.x - 2, SizeInPatches.y - 1);
                screenLocation = patchPosition * Patch + screenOffset;
                sourcePatch = new Vector2Int(3, 2);
                game.DrawPartialSprite(screenLocation, sprite, sourcePatch * Patch, PatchSize.x, PatchSize.y);
            }

            // Draw visible items.
            for (var i = 0; i < visibleItems; i++)
            {
                // Cell location.
                cell = new Vector2Int(i % CellTable.x, i / CellTable.x);

                // Patch location, including border offset and padding.
                patchPosition = new Vector2Int(cell.x * (CellSize.x + CellPadding.x) + 1, cell.y * (CellSize.y + CellPadding.y) + 1);

                // Actual screen location in pixels.
                screenLocation = patchPosition * Patch + screenOffset;

                // Display item header.
                game.DrawText(
                    screenLocation,
                    Items[topLeftItem + i].Name,
                    Items[topLeftItem + i].Enabled ? Pixel.Presets.White : Pixel.Presets.DarkGrey
                );

                // Display indicator that panel has a sub-panel.
                if (Items[topLeftItem + i].HasChildren())
                {
                    patchPosition = new Vector2Int(
                        cell.x * (CellSize.x + CellPadding.x) + 1 + CellSize.x,
                        cell.y * (CellSize.y + CellPadding.y) + 1
                    );
                    sourcePatch = new Vector2Int(3, 1);
                    screenLocation = patchPosition * Patch + screenOffset;
                    game.DrawPartialSprite(screenLocation, sprite, sourcePatch * Patch, PatchSize.x, PatchSize.y);
                }
            }

            #endregion === Draw Panel Contents ===

            // Calculate cursor position in screen space in case system draws it.
            CursorPos = new Vector2Int(
                //var cursorPosX = (CellCursor.x * (CellSize.x + CellPadding.x)) * Patch + screenOffset.x - Patch;
                CellCursor.x * (CellSize.x + CellPadding.x) * Patch + screenOffset.x - Patch,
                //var cursorPosY = ((CellCursor.y - TopVisibleRow) * (CellSize.y + CellPadding.y)) * Patch + screenOffset.y + Patch;
                (CellCursor.y - TopVisibleRow) * (CellSize.y + CellPadding.y) * Patch + screenOffset.y + Patch
            );

            // Set the PixelMode back to the "current" pixel mode.
            game.PixelMode = currentPixelMode;
        }

        public MenuObject OnConfirm()
        {
            if (Items[CursorItem].HasChildren()) return Items[CursorItem];
            else return this;
        }

        public void OnDown()
        {
            CellCursor = new Vector2Int(CellCursor.x, CellCursor.y == TotalRows ? TotalRows - 1 : CellCursor.y + 1);

            if (CellCursor.y > (TopVisibleRow + CellTable.y - 1))
            {
                TopVisibleRow++;
                if (TopVisibleRow > (TotalRows - CellTable.y)) TopVisibleRow = TotalRows - CellTable.y;
            }

            ClampCursor();
        }

        public void OnLeft()
        {
            CellCursor = new Vector2Int(CellCursor.x - 1 < 0 ? 0 : CellCursor.x - 1, CellCursor.y);
            ClampCursor();
        }

        public void OnRight()
        {
            CellCursor = new Vector2Int(CellCursor.x + 1 == CellTable.x ? CellTable.x - 1 : CellCursor.x + 1, CellCursor.y);
            ClampCursor();
        }

        public void OnUp()
        {
            CellCursor = new Vector2Int(CellCursor.x, CellCursor.y - 1 < 0 ? 0 : CellCursor.y - 1);

            if (CellCursor.y < TopVisibleRow)
            {
                TopVisibleRow--;
                if (TopVisibleRow < 0) TopVisibleRow = 0;
            }

            ClampCursor();
        }

        #endregion Actions
    }
}