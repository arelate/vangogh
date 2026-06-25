package compton_fragments

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/boggydigital/redux"
	"golang.org/x/net/html"

	"slices"
)

func FormatQuery(q map[string][]string, rdx redux.Readable) map[string][]string {

	fq := make(map[string][]string)

	for p, vals := range q {
		for _, val := range vals {
			val = html.EscapeString(val)
			if pv, ok := compton_data.PropertyTitles[val]; ok {
				fq[p] = append(fq[p], pv)
			} else if slices.Contains(compton_data.BinaryDigestProperties, p) {
				fq[p] = append(fq[p], compton_data.BinaryTitles[val])
			} else {
				switch p {
				case vangogh_integration.OperatingSystemsProperty:
					fq[p] = append(fq[p], compton_data.OperatingSystemTitles[val])
				case vangogh_integration.GogTagIdProperty:
					if tn, sure := rdx.GetLastVal(vangogh_integration.GogTagNameProperty, val); sure {
						fq[p] = append(fq[p], tn)
					} else {
						fq[p] = append(fq[p], val)
					}
				default:
					fq[p] = append(fq[p], val)
				}
			}
		}
	}
	return fq
}
