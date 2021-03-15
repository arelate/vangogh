package cmd

import (
	"fmt"
	"github.com/arelate/gog_types"
)

func Search(mt gog_types.Media, text, imageId string) error {

	fmt.Println("text:", text, "image-id:", imageId)

	//propStashes, err := vangogh_properties.PropStashes(ps)
	//if err != nil {
	//	return err
	//}
	//
	//if len(pts) == 0 {
	//	pts = vangogh_types.AllLocalProductTypes()
	//}
	//
	//for _, pt := range pts {
	//
	//	matchIds := make([]string, 0)
	//
	//	vr, err := vangogh_values.NewReader(pt, mt)
	//	if err != nil {
	//		return err
	//	}
	//
	//	for _, id := range vr.All() {
	//		matchesAll := true
	//
	//		for _, pp := range ps {
	//
	//			stash := propStashes[pp]
	//
	//			val, ok := stash.Get(id)
	//			if !ok {
	//				matchesAll = false
	//			}
	//
	//			matchesValue := false
	//			for _, req := range query[pp] {
	//				if strings.Contains(strings.ToLower(val), req) {
	//					matchesValue = true
	//				}
	//			}
	//
	//			matchesAll = matchesAll && matchesValue
	//
	//			if !matchesAll {
	//				break
	//			}
	//		}
	//
	//		if matchesAll {
	//			matchIds = append(matchIds, id)
	//		}
	//	}
	//
	//	if len(matchIds) > 0 {
	//		fmt.Printf("%s (%s):\n", pt, mt)
	//		if err := List(matchIds, pt, mt, properties...); err != nil {
	//			return err
	//		}
	//	}
	//}

	return nil
}
