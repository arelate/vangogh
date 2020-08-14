package filenames

import (
	"fmt"
)

func Product(id int) string {
	return fmt.Sprintf("%v.json", id)
}
