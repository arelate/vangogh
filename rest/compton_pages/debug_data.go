package compton_pages

import (
	"bytes"
	"encoding/json"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_styles"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/kevlar"
	"io"
	"strings"
)

func DebugData(id string, pt vangogh_integration.ProductType) (compton.PageElement, error) {

	absPtDir, err := vangogh_integration.AbsProductTypeDir(pt)
	if err != nil {
		return nil, err
	}

	ext := kevlar.JsonExt
	if pt == vangogh_integration.PcgwRaw {
		ext = ".txt"
	}

	kv, err := kevlar.New(absPtDir, ext)
	if err != nil {
		return nil, err
	}

	title := compton_data.TypesTitles[pt.String()] + " data for " + id
	p := compton.IframeExpandContent(pt.String(), title)
	p.RegisterStyles(compton_styles.Styles, "debug.css")

	ptContent, err := kv.Get(id)
	if err != nil {
		return nil, err
	}

	element, err := formatJson(ptContent)
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
