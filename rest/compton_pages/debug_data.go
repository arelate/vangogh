package compton_pages

import (
	"bytes"
	"encoding/json"
	"io"
	"net/url"
	"strings"
	"time"

	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/opencritic_integration"
	"github.com/arelate/southern_light/pcgw_integration"
	"github.com/arelate/southern_light/protondb_integration"
	"github.com/arelate/southern_light/steam_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/southern_light/wikipedia_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_styles"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
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

	var pageProperty string

	switch pt {
	case vangogh_integration.OrderPage:
		pageProperty = vangogh_integration.OrderPageProductsProperty
	case vangogh_integration.AccountPage:
		pageProperty = vangogh_integration.AccountPageProductsProperty
	case vangogh_integration.CatalogPage:
		pageProperty = vangogh_integration.CatalogPageProductsProperty
	default:
		// do nothing
	}

	productOrPageId := id
	if pageProperty != "" {
		if pageId, ok := rdx.GetLastVal(pageProperty, id); ok {
			productOrPageId = pageId
		}
	}

	ptId, err := vangogh_integration.ProductTypeId(pt, productOrPageId)
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

	var urlFunc func(id string) *url.URL

	switch pt {
	case vangogh_integration.CatalogPage:
		urlFunc = gog_integration.CatalogPageUrl
	case vangogh_integration.AccountPage:
		urlFunc = gog_integration.AccountPageUrl
	case vangogh_integration.ApiProducts:
		urlFunc = gog_integration.ApiProductUrl
	case vangogh_integration.OrderPage:
		urlFunc = gog_integration.OrdersPageUrl
	case vangogh_integration.Details:
		urlFunc = gog_integration.DetailsUrl
	case vangogh_integration.GamesDbGogProducts:
		urlFunc = gog_integration.GamesDbGogExternalReleaseUrl
	case vangogh_integration.SteamAppDetails:
		urlFunc = steam_integration.AppDetailsUrl
	case vangogh_integration.SteamAppNews:
		urlFunc = steam_integration.NewsForAppUrl
	case vangogh_integration.SteamAppReviews:
		urlFunc = steam_integration.AppReviewsUrl
	case vangogh_integration.SteamDeckCompatibilityReport:
		urlFunc = steam_integration.DeckAppCompatibilityReportUrl
	case vangogh_integration.PcgwGogPageId:
		urlFunc = pcgw_integration.GogPageIdCargoQueryUrl
	case vangogh_integration.PcgwSteamPageId:
		urlFunc = pcgw_integration.SteamPageIdCargoQueryUrl
	case vangogh_integration.PcgwRaw:
		urlFunc = pcgw_integration.WikiRawUrl
	case vangogh_integration.WikipediaRaw:
		urlFunc = wikipedia_integration.WikiRawUrl
	//case vangogh_integration.HltbData:
	//requires buildId
	//urlFunc = hltb_integration.DataUrl
	case vangogh_integration.ProtonDbSummary:
		urlFunc = protondb_integration.SummaryUrl
	case vangogh_integration.OpenCriticApiGame:
		urlFunc = opencritic_integration.ApiGameUrl
	}

	var productOrPageUrl string
	if urlFunc != nil {
		productOrPageUrl = urlFunc(productOrPageId).String()
	}

	hasGetDataProperty := false
	frow := compton.Frow(p).FontSize(size.Small)

	var lastUpdated time.Time
	if rdx.HasKey(vangogh_integration.GetDataLastUpdatedProperty, ptId) {
		var ok bool
		if lastUpdated, ok, err = rdx.ParseLastValTime(vangogh_integration.GetDataLastUpdatedProperty, ptId); ok && err == nil {
			frow.PropVal("Last Updated", lastUpdated.Local().Format(time.RFC1123))
			hasGetDataProperty = true
		} else if err != nil {
			return nil, err
		}
	}

	if errorDate, ok, err := rdx.ParseLastValTime(vangogh_integration.GetDataErrorDateProperty, ptId); ok && err == nil {
		if errorDate.After(lastUpdated) {
			frow.PropVal("Error Date", errorDate.Local().Format(time.RFC1123))
			hasGetDataProperty = true
			if errorMsg, ok := rdx.GetLastVal(vangogh_integration.GetDataErrorMessageProperty, ptId); ok && errorMsg != "" {
				frow.PropVal("Error Message", errorMsg)
			}
		}
	} else if err != nil {
		return nil, err
	}

	if productOrPageUrl != "" {
		frow.PropLinkColor("Source", color.Cyan, "URL", productOrPageUrl)
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
