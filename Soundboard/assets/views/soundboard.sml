<lane orientation="vertical" horizontal-content-alignment="middle" layout="80%" *context={Soundboard}>
    <banner background={@Mods/StardewUI/Sprites/BannerBackground} background-border-thickness="48,0" padding="12" margin="0,8" text={SelectedCategory} click=|GoToNextCategory()| />
    <frame layout="stretch stretch" background={@Mods/StardewUI/Sprites/ControlBorder} margin="0,0,0,0" padding="32,32">
        <!-- <scrollable> -->
            <lane orientation="vertical" layout="stretch content">
                <scrollable layout="stretch">
                <grid *switch={SelectedCategory} layout="stretch content" item-layout="count: 2" item-spacing="32,0" horizontal-item-alignment="middle">
                    <frame *case="Music" *repeat={Music} +state:playing={IsPlaying} transform-origin="0.5,0.5" +state:playing:transform="scale: 0.95" focusable="true" padding="24, 16" margin="0, 12" vertical-content-alignment="middle" background={@Spiderbuttons.Soundboard/Sprites/UI:Border}>
                        <lane layout="stretch content" *switch={IsPlaying} margin="0,8" vertical-content-alignment="middle" click=|ToggleState()|>    
                            <image *case="false" layout="36px" margin="0,0,4,0" sprite={@Spiderbuttons.Soundboard/Sprites/UI:PlayButton} />
                            <image *case="true" layout="36px" margin="0,0,4,0" sprite={@Spiderbuttons.Soundboard/Sprites/UI:StopButton} />
                            <lane vertical-content-alignment="middle">
                                <label max-lines="1" margin="0,0,8,0" text={Id} />
                                <label margin="0,0,8,0" text={FormattedDuration} />
                                <image *if={IsModded} layout="32px" sprite={@Spiderbuttons.Soundboard/Sprites/UI:Pufferchick} />
                            </lane>
                            <!-- <spacer layout="stretch"/> -->
                            <!-- <lane opacity="0.65">
                                <label max-lines="1" margin="0,0,8,0" text={FormattedDuration} />
                            </lane> -->
                        </lane>
                    </frame>
                    <frame *case="Sound Effects" *repeat={SoundEffects} +state:playing={IsPlaying} transform-origin="0.5,0.5" +state:playing:transform="scale: 0.95" focusable="true" padding="24, 12" margin="0, 12" vertical-content-alignment="middle" background={@Spiderbuttons.Soundboard/Sprites/UI:Border}>
                        <lane layout="stretch content" *switch={IsPlaying} margin="0,8" vertical-content-alignment="middle" click=|ToggleState()|>    
                            <image *case="false" layout="36px" margin="0,0,4,0" sprite={@Spiderbuttons.Soundboard/Sprites/UI:PlayButton} />
                            <image *case="true" layout="36px" margin="0,0,4,0" sprite={@Spiderbuttons.Soundboard/Sprites/UI:StopButton} />
                            <lane>
                                <label max-lines="1" margin="0,0,8,0" text={Id} />
                            </lane>
                            <image *if={IsModded} layout="36px" sprite={@Spiderbuttons.Soundboard/Sprites/UI:Pufferchick} />
                        </lane>
                    </frame>
                    <frame *case="Ambient" *repeat={Ambient} +state:playing={IsPlaying} transform-origin="0.5,0.5" +state:playing:transform="scale: 0.95" focusable="true" padding="24, 12" margin="0, 12" vertical-content-alignment="middle" background={@Spiderbuttons.Soundboard/Sprites/UI:Border}>
                        <lane layout="stretch content" *switch={IsPlaying} margin="0,8" vertical-content-alignment="middle" click=|ToggleState()|>    
                            <image *case="false" layout="36px" margin="0,0,4,0" sprite={@Spiderbuttons.Soundboard/Sprites/UI:PlayButton} />
                            <image *case="true" layout="36px" margin="0,0,4,0" sprite={@Spiderbuttons.Soundboard/Sprites/UI:StopButton} />
                            <lane>
                                <label max-lines="1" margin="0,0,8,0" text={Id} />
                            </lane>
                            <lane opacity="0.65">
                                <label max-lines="1" margin="0,0,8,0" text={FormattedDuration} />
                            </lane>
                        </lane>
                    </frame>
                    <frame *case="Footsteps" *repeat={Footsteps} +state:playing={IsPlaying} transform-origin="0.5,0.5" +state:playing:transform="scale: 0.95" focusable="true" padding="24, 12" margin="0, 12" vertical-content-alignment="middle" background={@Spiderbuttons.Soundboard/Sprites/UI:Border}>
                        <lane layout="stretch content" *switch={IsPlaying} margin="0,8" vertical-content-alignment="middle" click=|ToggleState()|>    
                            <image *case="false" layout="36px" margin="0,0,4,0" sprite={@Spiderbuttons.Soundboard/Sprites/UI:PlayButton} />
                            <image *case="true" layout="36px" margin="0,0,4,0" sprite={@Spiderbuttons.Soundboard/Sprites/UI:StopButton} />
                            <lane>
                                <label max-lines="1" margin="0,0,8,0" text={Id} />
                            </lane>
                            <lane opacity="0.65">
                                <label max-lines="1" margin="0,0,8,0" text={FormattedDuration} />
                            </lane>
                        </lane>
                    </frame>
                </grid>
                </scrollable>
            </lane>
        <!-- </scrollable> -->
    </frame>
</lane>