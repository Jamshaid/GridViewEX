﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace GridViewEx
{
    public class ColumnEx : DataControlField
    {
        public string HeaderToolTip { get; set; }
        public SearchTypeEnum SearchType { get; set; }
        public DataFormatEnum DataFormat { get; set; }
        public string DataFormatExpression { get; set; }
        public string NullDisplayText { get; set; }
        public Color NullDisplayColor { get; set; }
        public bool NullDisplayBold { get; set; }
        public string NavigateUrl { get; set; }
        public event EventHandler FilterApplied;
        public List<ListItem> DropDownDataSource { get; set; }

        public string DataField
        {
            get
            {
                object value = ViewState["DataField"];

                if (value != null)
                    return value.ToString();

                return string.Empty;
            }

            set
            {
                ViewState["DataField"] = value;
                OnFieldChanged();
            }
        }

        protected override DataControlField CreateField()
        {
            return new BoundField();
        }

        public override void InitializeCell(DataControlFieldCell cell, DataControlCellType cellType, DataControlRowState rowState, int rowIndex)
        {
            base.InitializeCell(cell, cellType, rowState, rowIndex);

            if (cellType == DataControlCellType.Header)
            {
                var lb = new LinkButton
                {
                    Text = HeaderText,
                    ToolTip = String.IsNullOrWhiteSpace(HeaderToolTip) ? HeaderText : HeaderToolTip,
                    CommandName = "Sort",
                    CommandArgument = DataField
                };

                var tt = cell.Controls;
                cell.Controls.Add(lb);

                if (SearchType != SearchTypeEnum.None)
                {
                    // Add filter control
                    switch (SearchType)
                    {
                        case SearchTypeEnum.TextBox:
                            cell.Controls.Add(CreateFilterTextBoxControl());
                            break;
                        case SearchTypeEnum.DropDownList:
                            cell.Controls.Add(CreateFilterDropDownListControl());
                            break;
                    }
                }
            }
            else if (cellType == DataControlCellType.DataCell)
                cell.DataBinding += new EventHandler(cell_DataBinding);
        }

        protected void cell_DataBinding(object sender, EventArgs e)
        {
            var cell = (TableCell)sender;
            var dataItem = DataBinder.GetDataItem(cell.NamingContainer);
            var dataValue = DataBinder.GetPropertyValue(dataItem, DataField);
            string value = dataValue != null ? dataValue.ToString() : "";
            if (!String.IsNullOrWhiteSpace(value))
                switch (DataFormat)
                {
                    case DataFormatEnum.Percentage:
                        Decimal pValue;
                        if (Decimal.TryParse(value, out pValue))
                        {
                            var pText = pValue % 1 == 0 ? String.Format("{0:0%}", pValue) : String.Format("{0:0.00%}", pValue);

                            if (!String.IsNullOrWhiteSpace(NavigateUrl))
                                cell.Controls.Add(new HyperLink { Text = pText, NavigateUrl = NavigateUrl, ToolTip = pText });
                            else
                                cell.Text = pText;
                        }
                        else
                        {
                            if (!String.IsNullOrWhiteSpace(NavigateUrl))
                                cell.Controls.Add(new HyperLink { Text = value, NavigateUrl = NavigateUrl, ToolTip = value });
                            else
                                cell.Text = value;
                        }
                        break;
                    case DataFormatEnum.Currency:
                        Decimal cValue;
                        if (Decimal.TryParse(value, out cValue))
                        {
                            var cText = cValue % 1 == 0 ? String.Format("{0:C0}", cValue) : String.Format("{0:C}", cValue);

                            if (!String.IsNullOrWhiteSpace(NavigateUrl))
                                cell.Controls.Add(new HyperLink { Text = cText, NavigateUrl = NavigateUrl, ToolTip = cText });
                            else
                                cell.Text = cText;
                        }
                        else
                        {
                            if (!String.IsNullOrWhiteSpace(NavigateUrl))
                                cell.Controls.Add(new HyperLink { Text = value, NavigateUrl = NavigateUrl, ToolTip = value });
                            else
                                cell.Text = value;
                        }
                        break;
                    case DataFormatEnum.Date:
                        DateTime dValue;
                        if (DateTime.TryParse(value, out dValue))
                        {
                            var dText = dValue.ToShortDateString();

                            if (!String.IsNullOrWhiteSpace(NavigateUrl))
                                cell.Controls.Add(new HyperLink { Text = dText, NavigateUrl = NavigateUrl, ToolTip = dText });
                            else
                                cell.Text = dText;
                        }
                        else
                        {
                            if (!String.IsNullOrWhiteSpace(NavigateUrl))
                                cell.Controls.Add(new HyperLink { Text = value, NavigateUrl = NavigateUrl, ToolTip = value });
                            else
                                cell.Text = value;
                        }
                        break;
                    case DataFormatEnum.ShortDate:
                        DateTime sdValue;
                        if (DateTime.TryParse(value, out sdValue))
                        {
                            var sdText = String.Format("{0:MM/dd}", sdValue);

                            if (!String.IsNullOrWhiteSpace(NavigateUrl))
                                cell.Controls.Add(new HyperLink { Text = sdText, NavigateUrl = NavigateUrl, ToolTip = sdText });
                            else
                                cell.Text = sdText;
                        }
                        else
                        {
                            if (!String.IsNullOrWhiteSpace(NavigateUrl))
                                cell.Controls.Add(new HyperLink { Text = value, NavigateUrl = NavigateUrl, ToolTip = value });
                            else
                                cell.Text = value;
                        }
                        break;
                    case DataFormatEnum.Hour:
                        Decimal hValue;
                        if (Decimal.TryParse(value, out hValue))
                        {
                            var hText = hValue % 1 == 0 ? String.Format("{0:0 H}", hValue) : String.Format("{0:0.00 H}", hValue);

                            if (!String.IsNullOrWhiteSpace(NavigateUrl))
                                cell.Controls.Add(new HyperLink { Text = hText, NavigateUrl = NavigateUrl, ToolTip = hText });
                            else
                                cell.Text = hText;
                        }
                        else
                        {
                            if (!String.IsNullOrWhiteSpace(NavigateUrl))
                                cell.Controls.Add(new HyperLink { Text = value, NavigateUrl = NavigateUrl, ToolTip = value });
                            else
                                cell.Text = value;
                        }
                        break;
                    case DataFormatEnum.Expression:
                        if (!String.IsNullOrWhiteSpace(DataFormatExpression))
                        {
                            if (!String.IsNullOrWhiteSpace(NavigateUrl))
                                cell.Controls.Add(new HyperLink { Text = String.Format(DataFormatExpression, value), NavigateUrl = NavigateUrl, ToolTip = String.Format(DataFormatExpression, value) });
                            else
                                cell.Text = String.Format(DataFormatExpression, value);
                        }
                        else
                        {
                            if (!String.IsNullOrWhiteSpace(NavigateUrl))
                                cell.Controls.Add(new HyperLink { Text = value, NavigateUrl = NavigateUrl, ToolTip = value });
                            else
                                cell.Text = value;
                        }
                        break;
                    default:
                        if (!String.IsNullOrWhiteSpace(NavigateUrl))
                            cell.Controls.Add(new HyperLink { Text = value, NavigateUrl = NavigateUrl, ToolTip = value });
                        else
                            cell.Text = value;
                        break;
                }
            else
            {
                cell.Text = NullDisplayText;
                cell.ForeColor = NullDisplayColor;
                cell.Font.Bold = NullDisplayBold;
            }
        }

        #region CONTROL CREATION
        private Control CreateFilterTextBoxControl()
        {
            var controlClientID = this.Control.ClientID;
            var controlClientIDDataField = controlClientID + DataField;

            var divFilter = new HtmlGenericControl("div");
            divFilter.Attributes.Add("class", controlClientID + "Filters");
            divFilter.Attributes.Add("style", "display: none;");

            var divInputPrepend = new HtmlGenericControl("div");
            divInputPrepend.Attributes.Add("class", "input-prepend");

            var divBtnGroup = new HtmlGenericControl("div");
            divBtnGroup.Attributes.Add("class", "btn-group");

            var btnFilter = new HtmlButton();
            btnFilter.Attributes.Add("class", "btn dropdown-toggle");
            btnFilter.Attributes.Add("title", "Filter By");
            btnFilter.Attributes.Add("data-toggle", "dropdown");
            btnFilter.InnerHtml = "<i class=\"icon-filter\"></i>";
            divBtnGroup.Controls.Add(btnFilter);

            var ulFilter = new HtmlGenericControl("ul");
            ulFilter.Attributes.Add("class", "dropdown-menu");

            var txtBox = new TextBox
            {
                ID = "txt" + controlClientIDDataField,
                ClientIDMode = ClientIDMode.Static,
                AutoPostBack = true,
                CssClass = "span1"
            };

            var hiddenField = new HiddenField
            {
                ID = "hf" + controlClientIDDataField,
                ClientIDMode = ClientIDMode.Static
            };

            var liFilter = new HtmlGenericControl("li");
            liFilter.InnerHtml = "<a href=\"#\" onclick=\"" + controlClientID + "SaveSearchExp('" + hiddenField.ClientID + "', '" + txtBox.ClientID + "', '= ');\">Is equal to</a>";
            ulFilter.Controls.Add(liFilter);

            liFilter = new HtmlGenericControl("li");
            liFilter.InnerHtml = "<a href=\"#\" onclick=\"" + controlClientID + "SaveSearchExp('" + hiddenField.ClientID + "', '" + txtBox.ClientID + "', '!= ');\">Is not equal to</a>";
            ulFilter.Controls.Add(liFilter);

            // Check the data format to add the correct filter expressions
            if (DataFormat == DataFormatEnum.Number
                || DataFormat == DataFormatEnum.Currency
                || DataFormat == DataFormatEnum.Hour
                || DataFormat == DataFormatEnum.Percentage
                || DataFormat == DataFormatEnum.Date
                || DataFormat == DataFormatEnum.ShortDate)
            {
                liFilter = new HtmlGenericControl("li");
                liFilter.InnerHtml = "<a href=\"#\" onclick=\"" + controlClientID + "SaveSearchExp('" + hiddenField.ClientID + "', '" + txtBox.ClientID + "', '> ');\">Is greater than</a>";
                ulFilter.Controls.Add(liFilter);

                liFilter = new HtmlGenericControl("li");
                liFilter.InnerHtml = "<a href=\"#\" onclick=\"" + controlClientID + "SaveSearchExp('" + hiddenField.ClientID + "', '" + txtBox.ClientID + "', '>= ');\">Is greater than or equal to</a>";
                ulFilter.Controls.Add(liFilter);

                liFilter = new HtmlGenericControl("li");
                liFilter.InnerHtml = "<a href=\"#\" onclick=\"" + controlClientID + "SaveSearchExp('" + hiddenField.ClientID + "', '" + txtBox.ClientID + "', '< ');\">Is less than</a>";
                ulFilter.Controls.Add(liFilter);

                liFilter = new HtmlGenericControl("li");
                liFilter.InnerHtml = "<a href=\"#\" onclick=\"" + controlClientID + "SaveSearchExp('" + hiddenField.ClientID + "', '" + txtBox.ClientID + "', '<= ');\">Is less than or equal to</a>";
                ulFilter.Controls.Add(liFilter);
            }
            else
            {
                liFilter = new HtmlGenericControl("li");
                liFilter.InnerHtml = "<a href=\"#\" onclick=\"" + controlClientID + "SaveSearchExp('" + hiddenField.ClientID + "', '" + txtBox.ClientID + "', '* ');\">Contains</a>";
                ulFilter.Controls.Add(liFilter);

                liFilter = new HtmlGenericControl("li");
                liFilter.InnerHtml = "<a href=\"#\" onclick=\"" + controlClientID + "SaveSearchExp('" + hiddenField.ClientID + "', '" + txtBox.ClientID + "', '!* ');\">Not contains</a>";
                ulFilter.Controls.Add(liFilter);

                liFilter = new HtmlGenericControl("li");
                liFilter.InnerHtml = "<a href=\"#\" onclick=\"" + controlClientID + "SaveSearchExp('" + hiddenField.ClientID + "', '" + txtBox.ClientID + "', '˄ ');\">Starts with</a>";
                ulFilter.Controls.Add(liFilter);

                liFilter = new HtmlGenericControl("li");
                liFilter.InnerHtml = "<a href=\"#\" onclick=\"" + controlClientID + "SaveSearchExp('" + hiddenField.ClientID + "', '" + txtBox.ClientID + "', '˅ ');\">Ends with</a>";
                ulFilter.Controls.Add(liFilter);

                liFilter = new HtmlGenericControl("li");
                liFilter.InnerHtml = "<a href=\"#\" onclick=\"" + controlClientID + "SaveSearchExp('" + hiddenField.ClientID + "', '" + txtBox.ClientID + "', '!˄ ');\">Not starts with</a>";
                ulFilter.Controls.Add(liFilter);

                liFilter = new HtmlGenericControl("li");
                liFilter.InnerHtml = "<a href=\"#\" onclick=\"" + controlClientID + "SaveSearchExp('" + hiddenField.ClientID + "', '" + txtBox.ClientID + "', '!˅ ');\">Not ends with</a>";
                ulFilter.Controls.Add(liFilter);
            }

            divBtnGroup.Controls.Add(ulFilter);
            divInputPrepend.Controls.Add(divBtnGroup);

            txtBox.TextChanged += new EventHandler(txtBox_TextChanged);
            divInputPrepend.Controls.Add(txtBox);
            divInputPrepend.Controls.Add(hiddenField);

            ((GridViewEx)this.Control).JSScript += (DataFormat == DataFormatEnum.Date || DataFormat == DataFormatEnum.ShortDate)
                ? @"
                function " + controlClientIDDataField + @"Function() {
                    $('#" + txtBox.ClientID + @"').datepicker().on('changeDate', function (ev) {
                        var date = new Date(ev.date);
                        $('#" + hiddenField.ClientID + @"').val($('#" + hiddenField.ClientID + @"').val() + date.toLocaleDateString());

                        $('#" + txtBox.ClientID + @"').datepicker('hide');
                        $('#" + txtBox.ClientID + @"').change();
                    });
                }"
                : @"
                function " + controlClientIDDataField + @"Function() {
                    $('#" + txtBox.ClientID + @"').removeAttr('onkeypress');
                    var oldOnChange = $('#" + txtBox.ClientID + @"').attr('onchange');
                    var onChange = '$(\'#" + hiddenField.ClientID + @"\').val($(\'#" + hiddenField.ClientID + @"\').val() + $(\'#" + txtBox.ClientID + @"\').val());';
                    $('#" + txtBox.ClientID + @"').attr('onchange', onChange + oldOnChange);
                }";

            ((GridViewEx)this.Control).JSScriptEndRequestHandler += controlClientIDDataField + @"Function();";
            ((GridViewEx)this.Control).JSScriptDocumentReady += controlClientIDDataField + @"Function();";

            divFilter.Controls.Add(divInputPrepend);

            return divFilter;
        }

        private Control CreateFilterDropDownListControl()
        {
            var controlClientID = this.Control.ClientID;
            var controlClientIDDataField = controlClientID + DataField;

            var divFilter = new HtmlGenericControl("div");
            divFilter.Attributes.Add("class", controlClientID + "Filters");
            divFilter.Attributes.Add("style", "display: none;");

            var divInputPrepend = new HtmlGenericControl("div");
            divInputPrepend.Attributes.Add("class", "input-prepend");

            var divBtnGroup = new HtmlGenericControl("div");
            divBtnGroup.Attributes.Add("class", "btn-group");

            var btnFilter = new HtmlButton();
            btnFilter.Attributes.Add("class", "btn dropdown-toggle");
            btnFilter.Attributes.Add("title", "Filter By");
            btnFilter.Attributes.Add("data-toggle", "dropdown");
            btnFilter.InnerHtml = "<i class=\"icon-filter\"></i>";
            divBtnGroup.Controls.Add(btnFilter);

            var ulFilter = new HtmlGenericControl("ul");
            ulFilter.Attributes.Add("class", "dropdown-menu");

            var ddlDropDownList = new DropDownList
            {
                ID = "ddl" + controlClientID + DataField,
                ClientIDMode = ClientIDMode.Static,
                AutoPostBack = true,
                CssClass = "span1"
            };
            ddlDropDownList.SelectedIndexChanged += new EventHandler(ddlDropDownList_SelectedIndexChanged);

            if (DropDownDataSource != null)
                ddlDropDownList.DataSource = DropDownDataSource;

            var hiddenField = new HiddenField
            {
                ID = "hf" + controlClientIDDataField,
                ClientIDMode = ClientIDMode.Static
            };

            var liFilter = new HtmlGenericControl("li");
            liFilter.InnerHtml = "<a href=\"#\" onclick=\"" + controlClientID + "SaveSearchExp('" + hiddenField.ClientID + "', '" + ddlDropDownList.ClientID + "', '= ');\">Is equal to</a>";
            ulFilter.Controls.Add(liFilter);

            liFilter = new HtmlGenericControl("li");
            liFilter.InnerHtml = "<a href=\"#\" onclick=\"" + controlClientID + "SaveSearchExp('" + hiddenField.ClientID + "', '" + ddlDropDownList.ClientID + "', '!= ');\">Is not equal to</a>";
            ulFilter.Controls.Add(liFilter);

            // Check the data format to add the correct filter expressions
            if (DataFormat == DataFormatEnum.Number
                || DataFormat == DataFormatEnum.Currency
                || DataFormat == DataFormatEnum.Hour
                || DataFormat == DataFormatEnum.Percentage
                || DataFormat == DataFormatEnum.Date
                || DataFormat == DataFormatEnum.ShortDate)
            {
                liFilter = new HtmlGenericControl("li");
                liFilter.InnerHtml = "<a href=\"#\" onclick=\"" + controlClientID + "SaveSearchExp('" + hiddenField.ClientID + "', '" + ddlDropDownList.ClientID + "', '> ');\">Is greater than</a>";
                ulFilter.Controls.Add(liFilter);

                liFilter = new HtmlGenericControl("li");
                liFilter.InnerHtml = "<a href=\"#\" onclick=\"" + controlClientID + "SaveSearchExp('" + hiddenField.ClientID + "', '" + ddlDropDownList.ClientID + "', '>= ');\">Is greater than or equal to</a>";
                ulFilter.Controls.Add(liFilter);

                liFilter = new HtmlGenericControl("li");
                liFilter.InnerHtml = "<a href=\"#\" onclick=\"" + controlClientID + "SaveSearchExp('" + hiddenField.ClientID + "', '" + ddlDropDownList.ClientID + "', '< ');\">Is less than</a>";
                ulFilter.Controls.Add(liFilter);

                liFilter = new HtmlGenericControl("li");
                liFilter.InnerHtml = "<a href=\"#\" onclick=\"" + controlClientID + "SaveSearchExp('" + hiddenField.ClientID + "', '" + ddlDropDownList.ClientID + "', '<= ');\">Is less than or equal to</a>";
                ulFilter.Controls.Add(liFilter);
            }

            divBtnGroup.Controls.Add(ulFilter);
            divInputPrepend.Controls.Add(divBtnGroup);

            divInputPrepend.Controls.Add(ddlDropDownList);
            divInputPrepend.Controls.Add(hiddenField);

            ((GridViewEx)this.Control).JSScript += @"
                function " + this.Control.ClientID + DataField + @"Function() {
                    var oldOnChange = $('#" + ddlDropDownList.ClientID + @"').attr('onchange');
                    var onChange = '$(\'#" + hiddenField.ClientID + @"\').val($(\'#" + hiddenField.ClientID + @"\').val() + $(\'#" + ddlDropDownList.ClientID + @" option:selected\').val());';
                    $('#" + ddlDropDownList.ClientID + @"').attr('onchange', onChange + oldOnChange);
                }";

            ((GridViewEx)this.Control).JSScriptEndRequestHandler += controlClientIDDataField + @"Function();";
            ((GridViewEx)this.Control).JSScriptDocumentReady += controlClientIDDataField + @"Function();";

            divFilter.Controls.Add(divInputPrepend);

            return divFilter;
        }
        #endregion

        #region ELEMENT EVENTS
        protected void txtBox_TextChanged(object sender, EventArgs e)
        {
            var txt = sender as TextBox;
            if (txt != null)
            {
                var hiddenField = txt.NamingContainer.FindControl("hf" + this.Control.ClientID + DataField) as HiddenField;
                if (hiddenField != null)
                    ApplyFilter(hiddenField.Value);
            }
        }

        protected void ddlDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var ddl = (DropDownList)sender;
            if (ddl != null)
            {
                var hiddenField = ddl.NamingContainer.FindControl("hf" + this.Control.ClientID + DataField) as HiddenField;
                if (hiddenField != null)
                    ApplyFilter(hiddenField.Value);
            }
        }
        #endregion

        private void ApplyFilter(string fullFilterExpression)
        {
            var filter = new List<string>(fullFilterExpression.Trim().Split(' '));

            // Default values of the filter expression depending on the data format
            string filterExpression = String.Empty;
            switch (DataFormat)
            {
                case DataFormatEnum.Number:
                    filterExpression = "=";
                    break;
                case DataFormatEnum.Currency:
                    filterExpression = "=";
                    break;
                case DataFormatEnum.Date:
                    filterExpression = "=";
                    break;
                case DataFormatEnum.Hour:
                    filterExpression = "=";
                    break;
                case DataFormatEnum.Percentage:
                    filterExpression = "=";
                    break;
                case DataFormatEnum.ShortDate:
                    filterExpression = "=";
                    break;
                default:
                    filterExpression = SearchType == SearchTypeEnum.DropDownList
                        ? "="
                        : "˄";
                    break;
            }

            string filterText = String.Empty;

            if (Extensions.IsValidExpressionType(filter[0]))
            {
                filterExpression = filter[0];
                filter.Remove(filter[0]); // Remove filterExpression from the list, rest is text
                filterText = String.Join(" ", filter);
            }
            else
                filterText = String.Join(" ", filter);

            // Convert the number if needed
            switch (DataFormat)
            {
                case DataFormatEnum.Percentage:
                    filterText = (Decimal.Parse(filterText.Split('%')[0]) / 100M).ToString();
                    break;
                case DataFormatEnum.Currency:
                    Decimal cValue;
                    if (Decimal.TryParse(filterText, NumberStyles.Currency, CultureInfo.CurrentCulture, out cValue))
                        filterText = cValue.ToString();
                    break;
                case DataFormatEnum.Date:
                    DateTime dValue;
                    if (DateTime.TryParse(filterText, out dValue))
                        filterText = dValue.ToString();
                    break;
                case DataFormatEnum.ShortDate:
                    DateTime sdValue;
                    if (DateTime.TryParse(filterText, out sdValue))
                        filterText = sdValue.ToString();
                    break;
                case DataFormatEnum.Hour:
                    filterText = (Decimal.Parse(filterText.Split('H')[0])).ToString();
                    break;
                //case DataFormatEnum.Expression:
                //    text = (!String.IsNullOrWhiteSpace(dataFormatExpression))
                //        ? String.Format(dataFormatExpression, item.ToString())
                //        : item.ToString();
                //    break;
                //default:
                //    text = item.ToString();
                //    break;
            }

            if (!string.IsNullOrWhiteSpace(filterText))
            {
                var filters = new List<FilterExpression>();
                if (Control.Page.Session[this.Control.ID + "_Filters"] != null)
                    filters = Control.Page.Session[this.Control.ID + "_Filters"] as List<FilterExpression>;

                var filterExp = new FilterExpression
                {
                    Expression = Extensions.GetExpressionType(filterExpression),
                    ExpressionShortName = filterExpression,
                    Column = DataField,
                    DisplayName = this.HeaderText,
                    Text = filterText
                };
                if (!filters.Exists(x => x.Column == filterExp.Column
                    && x.Expression == filterExp.Expression
                    && x.ExpressionShortName == filterExp.ExpressionShortName
                    && x.Text == filterExp.Text))
                    filters.Add(filterExp);

                Control.Page.Session[this.Control.ID + "_Filters"] = filters;
                Control.Page.Session[this.Control.ID + "_PageIndex"] = 0;

                if (FilterApplied != null)
                    FilterApplied(null, EventArgs.Empty);
            }
        }
    }
}