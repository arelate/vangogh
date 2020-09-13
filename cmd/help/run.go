package help

import "fmt"

func Run(args []string) error {
	fmt.Println(Cmd, args)
	return nil
}
