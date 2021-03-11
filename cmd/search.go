package cmd

import (
	"fmt"
	"github.com/arelate/gog_types"
)

func Search(mt gog_types.Media, query map[string][]string, properties []string) error {
	fmt.Println(mt, query, properties)
	return nil
}
