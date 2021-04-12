package cmd

import "fmt"

func GetVideos(ids []string, all bool) error {
	fmt.Println("get-videos", "ids", ids, "all", all)
	return nil
}
