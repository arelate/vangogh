package itemize

import (
	"fmt"
	"github.com/boggydigital/gost"
)

func printFoundAndAll(idSet gost.StrSet) []string {
	items := idSet.All()
	msg := "nothing found"
	if len(items) > 0 {
		msg = fmt.Sprintf("found %d", len(items))
	}
	fmt.Println(msg)
	return items
}
