package compton_pages

import (
	"bytes"
	"encoding/json"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_styles"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"io"
	"strings"
	"time"
)

func DebugData(id string, pt vangogh_integration.ProductType) (compton.PageElement, error) {

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return nil, err
	}

	rdx, err := redux.NewReader(reduxDir, vangogh_integration.GetDataProperties()...)
	if err != nil {
		return nil, err
	}

	ptId, err := vangogh_integration.ProductTypeId(pt, id)
	if err != nil {
		return nil, err
	}

	absPtDir, err := vangogh_integration.AbsProductTypeDir(pt)
	if err != nil {
		return nil, err
	}

	var ext string
	switch pt {
	case vangogh_integration.PcgwRaw:
		fallthrough
	case vangogh_integration.WikipediaRaw:
		ext = kevlar.TxtExt
	default:
		ext = kevlar.JsonExt
	}

	kv, err := kevlar.New(absPtDir, ext)
	if err != nil {
		return nil, err
	}

	title := compton_data.TypesTitles[pt.String()] + " data for " + id
	p := compton.IframeExpandContent(pt.String(), title)
	p.RegisterStyles(compton_styles.Styles, "debug.css")

	hasGetDataProperty := false
	frow := compton.Frow(p).FontSize(size.Small)

	if rdx.HasKey(vangogh_integration.GetDataLastUpdatedProperty, ptId) {
		if lastUpdated, ok, err := rdx.ParseLastValTime(vangogh_integration.GetDataLastUpdatedProperty, ptId); ok && err == nil {
			frow.PropVal("Last Updated", lastUpdated.Local().Format(time.RFC1123))
			hasGetDataProperty = true
		} else if err != nil {
			return nil, err
		}
	}

	if errorMsg, ok := rdx.GetLastVal(vangogh_integration.GetDataErrorMessageProperty, ptId); ok && errorMsg != "" {
		frow.PropVal("Error Message", errorMsg)
		if errorDate, ok, err := rdx.ParseLastValTime(vangogh_integration.GetDataErrorDateProperty, ptId); ok && err == nil {
			frow.PropVal("Error Date", errorDate.Local().Format(time.RFC1123))
		} else if err != nil {
			return nil, err
		}
		hasGetDataProperty = true
	}

	if hasGetDataProperty {
		p.Append(frow, compton.Break())
	}

	ptContent, err := kv.Get(id)
	if err != nil {
		return nil, err
	}

	var element compton.Element

	switch pt {
	case vangogh_integration.PcgwRaw:
		fallthrough
	case vangogh_integration.WikipediaRaw:
		element, err = preText(ptContent)
	default:
		element, err = formatJson(ptContent)
	}

	if err != nil {
		return nil, err
	}

	if element != nil {
		p.Append(element)
	}

	return p, nil
}

func formatJson(rc io.ReadCloser) (compton.Element, error) {

	var ptBuf bytes.Buffer
	if _, err := io.Copy(&ptBuf, rc); err != nil {
		return nil, err
	}
	defer rc.Close()

	var preBuf bytes.Buffer
	if err := json.Indent(&preBuf, ptBuf.Bytes(), "", "    "); err != nil {
		return nil, err
	}

	jsonString := preBuf.String()

	jsonString = strings.Replace(jsonString, "<", "&lt;", -1)
	jsonString = strings.Replace(jsonString, ">", "&gt;", -1)

	return compton.PreText(jsonString), nil
}

func preText(rc io.ReadCloser) (compton.Element, error) {
	var preBuf bytes.Buffer
	if _, err := io.Copy(&preBuf, rc); err != nil {
		return nil, err
	}
	defer rc.Close()

	textString := preBuf.String()

	textString = strings.Replace(textString, "<", "&lt;", -1)
	textString = strings.Replace(textString, ">", "&gt;", -1)

	return compton.PreText(textString), nil
}
