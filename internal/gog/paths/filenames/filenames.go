package filenames

import "fmt"

func GameDetails(id int) string {
	return fmt.Sprintf("%v.json", id)
}
