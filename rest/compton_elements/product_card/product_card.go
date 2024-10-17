package product_card

import (
	"bytes"
	_ "embed"
	"github.com/arelate/vangogh/rest/compton_atoms"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_elements/product_labels"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/compton/elements/els"
	"github.com/boggydigital/compton/elements/issa_image"
	"github.com/boggydigital/compton/elements/labels"
	"github.com/boggydigital/compton/elements/svg_use"
	"github.com/boggydigital/issa"
	"github.com/boggydigital/kevlar"
	"io"
	"slices"
	"strings"
)

const (
	registrationName      = "product-card"
	styleRegistrationName = "style-" + registrationName
)

var (
	//go:embed "markup/product-card.html"
	markupProductCard []byte
	//go:embed "style/product-card.css"
	styleProductCard []byte
)

type ProductCardElement struct {
	compton.BaseElement
	r         compton.Registrar
	poster    compton.Element
	osSymbols []compton.Element
	labels    *labels.LabelsElement
	rdx       kevlar.ReadableRedux
	id        string
}

func (pc *ProductCardElement) WriteStyles(w io.Writer) error {
	if pc.r.RequiresRegistration(styleRegistrationName) {
		if err := els.Style(styleProductCard, styleRegistrationName).WriteContent(w); err != nil {
			return err
		}
	}
	if pc.poster != nil {
		if err := pc.poster.WriteStyles(w); err != nil {
			return err
		}
	}
	if pc.labels != nil {
		if err := pc.labels.WriteStyles(w); err != nil {
			return err
		}
	}
	return nil
}

func (pc *ProductCardElement) WriteRequirements(w io.Writer) error {
	if pc.poster != nil {
		if err := pc.poster.WriteRequirements(w); err != nil {
			return err
		}
	}
	for _, symbol := range pc.osSymbols {
		if err := symbol.WriteRequirements(w); err != nil {
			return err
		}
	}
	return pc.BaseElement.WriteRequirements(w)
}

func (pc *ProductCardElement) WriteDeferrals(w io.Writer) error {
	if pc.poster != nil {
		return pc.poster.WriteDeferrals(w)
	}
	return nil
}

func (pc *ProductCardElement) WriteContent(w io.Writer) error {
	return compton.WriteContents(bytes.NewReader(markupProductCard), w, pc.elementFragmentWriter)
}

func (pc *ProductCardElement) elementFragmentWriter(t string, w io.Writer) error {
	switch t {
	case ".Id":
		if _, err := io.WriteString(w, pc.id); err != nil {
			return err
		}
	case ".Poster":
		if pc.poster != nil {
			if err := pc.poster.WriteContent(w); err != nil {
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
		if err := pc.labels.WriteContent(w); err != nil {
			return err
		}
	case ".OperatingSystems":
		for _, symbol := range pc.osSymbols {
			if err := symbol.WriteContent(w); err != nil {
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

func (pc *ProductCardElement) SetDehydratedPoster(dehydratedSrc, posterSrc string) *ProductCardElement {
	pc.poster = issa_image.IssaImageDehydrated(pc.r, dehydratedSrc, posterSrc)
	return pc
}

func (pc *ProductCardElement) SetHydratedPoster(hydratedSrc, posterSrc string) *ProductCardElement {
	pc.poster = issa_image.IssaImageHydrated(pc.r, hydratedSrc, posterSrc)
	return pc
}

func ProductCard(r compton.Registrar, id string, hydrated bool, rdx kevlar.ReadableRedux) *ProductCardElement {
	pc := &ProductCardElement{
		BaseElement: compton.BaseElement{
			TagName: compton_atoms.ProductCard,
			Markup:  markupProductCard,
		},
		r:   r,
		id:  id,
		rdx: rdx,
	}

	if viSrc, ok := rdx.GetLastVal(vangogh_local_data.VerticalImageProperty, id); ok {
		dhSrc, _ := rdx.GetLastVal(vangogh_local_data.DehydratedVerticalImageProperty, id)
		if hydrated {
			hSrc := issa.HydrateColor(dhSrc)
			pc.SetHydratedPoster(hSrc, "/image?id="+viSrc)
		} else {
			pc.SetDehydratedPoster(dhSrc, "/image?id="+viSrc)
		}
	}

	osOrder := []vangogh_local_data.OperatingSystem{
		vangogh_local_data.Windows,
		vangogh_local_data.MacOS,
		vangogh_local_data.Linux}
	if oses, ok := pc.rdx.GetAllValues(vangogh_local_data.OperatingSystemsProperty, pc.id); ok {
		pOses := vangogh_local_data.ParseManyOperatingSystems(oses)
		for _, os := range osOrder {
			if slices.Contains(pOses, os) {
				pc.osSymbols = append(pc.osSymbols, svg_use.SvgUse(pc.r, compton_data.OperatingSystemSymbols[os]))
			}
		}
	}

	pc.labels = labels.Labels(r,
		product_labels.FormatLabels(id, rdx, compton_data.LabelProperties...)...).
		FontSize(size.XSmall).
		ColumnGap(size.XXSmall).
		RowGap(size.XXSmall)

	pc.SetAttribute("data-id", id)

	return pc
}
