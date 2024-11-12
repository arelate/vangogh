package compton_fragments

import (
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/kevlar"
	"slices"
)

func FormatQuery(q map[string][]string, rdx kevlar.ReadableRedux) map[string][]string {

	fq := make(map[string][]string)

	for p, vals := range q {
		for _, val := range vals {
			if pv, ok := compton_data.PropertyTitles[val]; ok {
				fq[p] = append(fq[p], pv)
			} else if slices.Contains(compton_data.BinaryDigestProperties, p) {
				fq[p] = append(fq[p], compton_data.BinaryTitles[val])
			} else {
				switch p {
				case vangogh_local_data.TypesProperty:
					fq[p] = append(fq[p], compton_data.TypesTitles[val])
				case vangogh_local_data.OperatingSystemsProperty:
					fq[p] = append(fq[p], compton_data.OperatingSystemTitles[val])
				case vangogh_local_data.TagIdProperty:
					if tn, sure := rdx.GetLastVal(vangogh_local_data.TagNameProperty, val); sure {
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
