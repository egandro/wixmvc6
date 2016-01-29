<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:msxsl="urn:schemas-microsoft-com:xslt"
  xmlns:wix="http://schemas.microsoft.com/wix/2006/wi"
  xmlns:regex="http://mycompany.com/regular-expressions">
  
  <!-- from: http://www.itzinger.at/wp/?p=16 -->

  <!-- set output options -->
  <xsl:output method="xml" indent="yes"/>

  <!-- copy all to output -->
  <xsl:template match="@*|node()">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()"/>
    </xsl:copy>
  </xsl:template>

  <!-- Replace Source with "..\publish\WebApplication1" -->
  <xsl:template match="wix:File/@Source">
    <xsl:attribute name="Source">
      <xsl:value-of select="regex:replace(current(), 'SourceDir*', 'i', '..\publish\WebApplication1')"/>
    </xsl:attribute>
    <xsl:apply-templates select="@*|node()"/>
  </xsl:template>

  <!-- Regex replace function -->
  <msxsl:script language="JavaScript" implements-prefix="regex">
    <![CDATA[
      function replace(text, regex, flags, replace) {
        var sourceString= "";
        if (typeof(text) == "object"){
          if (text.Count > 0){
            text.MoveNext();
            sourceString= text.Current.Value;
          } else {
            return "";
          }
        } else{
          sourceString= text;
        }
        var re = new RegExp(regex, flags);
        return sourceString.replace(re, replace);
      }
      ]]>
  </msxsl:script>

</xsl:stylesheet>