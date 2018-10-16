namespace EventData

module StringUtils =

  let QUOTE = '"'

  // Get list of quoted strings
  let rec private getQuotedStrings_ (s:string) (posCurrent:int) list =
    let posStart = s.IndexOf(QUOTE, posCurrent)
    let posEnd = s.IndexOf(QUOTE, posStart + 1)
    if (posEnd <= 0) then
      List.rev list  // we put them in reverse
    else
      if (posStart < posEnd) then
        let currentQuotedStr = s.Substring(posStart + 1, (posEnd - (posStart +  1)))
        getQuotedStrings_ s (posEnd + 1) (currentQuotedStr::list)
      else
        list

  let getQuotedStrings (s:string) = getQuotedStrings_ s 0 []