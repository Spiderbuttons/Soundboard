<lane *context={:Soundboard}>
    <lane orientation="vertical" horizontal-content-alignment="middle" vertical-content-alignment="middle" layout="50% stretch" button-press=|HandleButton($Button)|>
        <lane vertical-content-alignment="middle">
            <image focusable="true" margin="4,0" sprite={@Mods/StardewUI/Sprites/LargeLeftArrow} click=|GoToNextCategory("-1")| />
            <banner layout="300px content" background={@Mods/StardewUI/Sprites/BannerBackground} background-border-thickness="48,0" padding="12" margin="0" text={CategoryName} />
            <image focusable="true" margin="4,0" sprite={@Mods/StardewUI/Sprites/LargeRightArrow} click=|GoToNextCategory("1")| />
        </lane>
        <frame *context={SelectedList} layout="stretch[800..] content[540..]" margin="0,12" padding="32" background={@Mods/StardewUI/Sprites/ControlBorder}>
            <scrollable peeking="64">
                <grid item-layout="count: 2" item-spacing="32,0" horizontal-item-alignment="middle">
                    <panel *repeat={CurrentPage}>
                        <frame  focusable="true" transform-origin="0.5,0.5" +transition:transform="100ms 0s easeOutCirc" transform={Transform} click=|~SoundList.ToggleSound(Id)| padding="24, 16" margin="0, 12" vertical-content-alignment="middle" background= {@Spiderbuttons.Soundboard/Sprites/UI:Border}>
                            <lane layout="stretch content" margin="0,8" vertical-content-alignment="middle" >
                                <image *!if={IsPlaying} layout="36px" margin="0,0,4,0" sprite={@Spiderbuttons.Soundboard/Sprites/UI:PlayButton} />
                                <image *if={IsPlaying} layout="36px" margin="0,0,4,0" sprite={@Spiderbuttons.Soundboard/Sprites/UI:StopButton} />
                                <image *if={:IsModded} z-index="69" padding="0,0,0,-8" tooltip={#Modded} layout="24px" margin="-32,-24,8,8" sprite={@Spiderbuttons.Soundboard/Sprites/UI:Pufferchick} />
                                <image *if={:DoesLoop} tooltip={#Loops} layout="24px" margin="-32,12,8,-12" transform="translateX: 1" sprite={@Spiderbuttons.Soundboard/Sprites/Cursors2:Loop} />
                                <label layout="stretch content" tooltip={Tooltip} margin="-4,0,8,0" max-lines="1" text={:Id} />
                                <label margin="8,0,0,0" opacity="0.65" text={:FormattedDuration} />
                            </lane>
                        </frame>
                    </panel>
                </grid>
            </scrollable>
        </frame>
        <lane layout="stretch content" *context={SelectedList}>
            <button *switch={AtMinPage} layout="250px content" click=|PrevPage()|>
                <label *case="false" text="Previous"/>
                <label *case="true" opacity="0.4" text="Previous"/>
            </button>
            <spacer layout="stretch"/>
            <button *switch={AtMaxPage} layout="250px content" click=|NextPage()|>
                <label *case="false" text="Next"/>
                <label *case="true" opacity="0.4" text="Next"/>
            </button>
        </lane>
    </lane>
</lane>