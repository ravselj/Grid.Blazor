﻿using GridBlazor.Columns;
using GridShared.Columns;
using Microsoft.AspNetCore.Components;
using System;

namespace GridBlazor.Pages
{
    public partial class GridCellComponent<T>
    {
        private const string TdStyle = "display:none;";

        private int _sequence = 0;
        protected string _cssStyles;
        protected string _cssClass;
        protected MarkupString _cell;
        protected Type _componentType;
        protected RenderFragment _cellRender;

        [Parameter]
        public IGridColumn Column { get; set; }

        [Parameter]
        public object Item { get; set; }

        [Parameter]
        public string TdClass { get; set; } = "grid-cell";

        protected override void OnParametersSet()
        {
            _componentType = ((GridColumnBase<T>)Column).ComponentType;
            if (_componentType != null)
                _cellRender = CreateComponent(_sequence, _componentType, Column, Item);
            else
                _cell = (MarkupString)Column.GetCell(Item).ToString();
            if (((GridColumnBase<T>)Column).Hidden)
                _cssStyles = ((GridStyledColumn)Column).GetCssStylesString() + " " + TdStyle;
            else
                _cssStyles = ((GridStyledColumn)Column).GetCssStylesString();
            _cssClass = ((GridStyledColumn)Column).GetCssClassesString() + " " + TdClass;
            string columnCssClasses = Column.GetCellCssClasses(Item);
            if(!string.IsNullOrWhiteSpace(columnCssClasses))
                _cssClass += " " + columnCssClasses;
        }

        public static RenderFragment CreateComponent(int sequence, Type componentType, IGridColumn column,
            object item)  => builder =>
        {
            if (componentType != null)
            {
                builder.OpenComponent(++sequence, componentType);
                builder.AddAttribute(++sequence, "Item", item);
                var gridProperty = componentType.GetProperty("Grid");
                if (gridProperty != null && gridProperty.PropertyType == typeof(CGrid<T>))
                    builder.AddAttribute(++sequence, "Grid", (CGrid<T>)column.ParentGrid);
                gridProperty = componentType.GetProperty("Actions");
                if (gridProperty != null)
                    builder.AddAttribute(++sequence, "Actions", ((GridColumnBase<T>)column).Actions);
                gridProperty = componentType.GetProperty("Functions");
                if (gridProperty != null)
                    builder.AddAttribute(++sequence, "Functions", ((GridColumnBase<T>)column).Functions);
                gridProperty = componentType.GetProperty("Object");
                if (gridProperty != null)
                    builder.AddAttribute(++sequence, "Object", ((GridColumnBase<T>)column).Object);
                builder.CloseComponent();
            }           
        };
    }
}
