package product_card

import (
	"bytes"
	"embed"
	_ "embed"
	"github.com/arelate/vangogh/rest/compton_atoms"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/issa"
	"github.com/boggydigital/kevlar"
	"io"
	"slices"
	"strings"
)

const (
	pcFilename = "style/product-card.css"
)

var (
	//go:embed "markup/product-card.html"
	markupProductCard []byte
	//go:embed "style/product-card.css"
	styleProductCard embed.FS
)

type ProductCardElement struct {
	*compton.BaseElement
	r         compton.Registrar
	poster    *compton.IssaImageElement
	osSymbols []compton.Element
	labels    *compton.LabelsElement
	rdx       kevlar.ReadableRedux
	id        string
}

func (pc *ProductCardElement) Write(w io.Writer) error {
	bts, err := pc.BaseElement.MarkupProvider.GetMarkup()
	if err != nil {
		return err
	}
	return compton.WriteContents(bytes.NewReader(bts), w, pc.elementFragmentWriter)
}

func (pc *ProductCardElement) elementFragmentWriter(t string, w io.Writer) error {
	switch t {
	case ".Id":
		if _, err := io.WriteString(w, pc.id); err != nil {
			return err
		}
	case ".Poster":
		if pc.poster != nil {
			if err := pc.poster.Write(w); err != nil {
				return err
			}
		}
	case ".Title":
		if title, ok := pc.rdx.GetLastVal(vangogh_local_data.TitleProperty, pc.id); ok {
			if _, err := io.WriteString(w, title); err != nil {
				return err
			}
		}
	case ".Labels":
		if err := pc.labels.Write(w); err != nil {
			return err
		}
	case ".OperatingSystems":
		for _, symbol := range pc.osSymbols {
			if err := symbol.Write(w); err != nil {
				return err
			}
		}
	case ".Developers":
		if developers, ok := pc.rdx.GetAllValues(vangogh_local_data.DevelopersProperty, pc.id); ok {
			if _, err := io.WriteString(w, strings.Join(developers, ", ")); err != nil {
				return err
			}
		}
	case ".Publishers":
		if publishers, ok := pc.rdx.GetAllValues(vangogh_local_data.PublishersProperty, pc.id); ok {
			if _, err := io.WriteString(w, strings.Join(publishers, ", ")); err != nil {
				return err
			}
		}

	case compton.AttributesToken:
		return pc.BaseElement.WriteFragment(compton.AttributesToken, w)
	default:
		return compton.ErrUnknownToken(t)
	}
	return nil
}

func ProductCard(r compton.Registrar, id string, hydrated bool, rdx kevlar.ReadableRedux) *ProductCardElement {
	pc := &ProductCardElement{
		BaseElement: compton.NewElement(compton.BytesMarkup(compton_atoms.ProductCard, markupProductCard)),
		r:           r,
		id:          id,
		rdx:         rdx,
	}

	if viSrc, ok := rdx.GetLastVal(vangogh_local_data.VerticalImageProperty, id); ok {
		dhSrc, _ := rdx.GetLastVal(vangogh_local_data.DehydratedVerticalImageProperty, id)
		if hydrated {
			hSrc := issa.HydrateColor(dhSrc)
			pc.poster = compton.IssaImageHydrated(pc.r, hSrc, "/image?id="+viSrc)
		} else {
			pc.poster = compton.IssaImageDehydrated(pc.r, dhSrc, "/image?id="+viSrc)
		}
		pc.poster.WidthPixels(85.5)
		pc.poster.HeightPixels(120.5)
	}

	osOrder := []vangogh_local_data.OperatingSystem{
		vangogh_local_data.Windows,
		vangogh_local_data.MacOS,
		vangogh_local_data.Linux}
	if oses, ok := pc.rdx.GetAllValues(vangogh_local_data.OperatingSystemsProperty, pc.id); ok {
		pOses := vangogh_local_data.ParseManyOperatingSystems(oses)
		for _, os := range osOrder {
			if slices.Contains(pOses, os) {
				pc.osSymbols = append(pc.osSymbols, compton.SvgUse(pc.r, compton_data.OperatingSystemSymbols[os]))
			}
		}
	}

	pc.labels = compton.Labels(r,
		FormatLabels(id, rdx, compton_data.LabelProperties...)...).
		FontSize(size.XSmall).
		ColumnGap(size.XXSmall).
		RowGap(size.XXSmall)

	pc.SetAttribute("data-id", id)

	r.RegisterStyles(styleProductCard, pcFilename)

	return pc
}
