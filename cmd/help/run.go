package help

import (
	"fmt"
	"github.com/boggydigital/vangogh/internal/gog/const/cmds"
)

func Run(args []string) error {
	fmt.Println(cmds.Help, args)
	return nil
}
